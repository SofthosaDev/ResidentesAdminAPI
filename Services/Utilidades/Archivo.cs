using WsAdminResidentes.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using WsAdminResidentes.Models.Utilidades;
using WsAdminResidentes.Services.Utilidades;
using WsAdminResidentes.Classes;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using WsAdminResidentes.Models.RespuestasBd;

namespace EvaluadorFinancieraWS.Services.Utilidades
{
    public interface IArchivosService
    {

        public ArchivoTemporal BuscarArchivoTemporal(Guid guid);

        public bool ExisteArchivo(Guid guid);

        public void EliminarAntiguos();

        public string MoverArchivo(string carpeta_destino, Guid guid);

        public void EliminarArchivo(Guid guid);

        public Guid SubirArchivo(IFormFile Archivo);

        public Task<ArchivoBdModel> SubirGoogleDrive(ArchivoTemporal archivo);

        public Task<string> GetGoogleDriveMiniatura(string id);
        public Task<Stream> GetGoogleDriveImagen(string id);
        public ArchivoBdModel GetArchivoBd(int id);
        public byte[] StreamToByteArray(Stream stream);
        public void AgregarFormato(string formato);
    }
    public class ArchivosService : IArchivosService
    {
        private readonly SqliteContext _dbContext;
        private readonly AppSettings _appSettings;
        public double tamanoMaximo { get; set; }

        private const double mb = 1048576;
        private readonly SqliteContext SqliteContext;
        public List<string> Formatos;
        private readonly IBaseDatosService _db;

        public ArchivosService(IBaseDatosService _db, IOptions<AppSettings> appSettings, int tamano_maximo = 50/*MB*/ )
        {
            this._db = _db;
            _appSettings = appSettings.Value as AppSettings;
            tamanoMaximo = 50 * mb;
            _dbContext = new SqliteContext(appSettings);
            SqliteContext = new SqliteContext(appSettings);
            Formatos = new List<string>();
        }

        public void EliminarAntiguos()
        {
            List<ArchivoTemporal> archivos_temporales = BuscarAntiguos();
            foreach (ArchivoTemporal archivo_temporal in archivos_temporales)
            {
                EliminarArchivo(archivo_temporal.guid);
            }
        }

        public string MoverArchivo(string carpeta_destino, Guid guid)
        {
            string destino_relativo = "";
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{carpeta_destino}")))
            {
                System.IO.Directory.CreateDirectory(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{carpeta_destino}")
                );
            }
            string temp = Path.GetRandomFileName();
            ArchivoTemporal archivo_temporal = BuscarArchivoTemporal(guid);
            destino_relativo = $"{carpeta_destino}/{temp}{archivo_temporal.Extension}";
            string ruta_absoluta_dest = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                destino_relativo
            );
            string ruta_absoluta_orig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                archivo_temporal.RutaRelativa
            );

            if (File.Exists(ruta_absoluta_orig))
            {
                File.Copy(ruta_absoluta_orig, ruta_absoluta_dest);
                //this.EliminarArchivo(archivo_temporal.guid);
            }
            else
            {
                throw (new DirectoryNotFoundException($@"El archivo con Id: {guid} no existe, 
                tal ves ha pasado demasiado tiempo desde su creación, verifica"));
            }
            EliminarAntiguos();
            return destino_relativo;
        }

        public void EliminarArchivo(Guid guid)
        {
            ArchivoTemporal archivo_temporal = this.BuscarArchivoTemporal(guid);
            string ruta_absoluta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                archivo_temporal.RutaRelativa
            );
            if (File.Exists(ruta_absoluta))
            {
                File.Delete(ruta_absoluta);
            }
            this.SqliteContext.ArchivosTemporales.Remove(archivo_temporal);
            this.SqliteContext.SaveChanges();
        }

        public Guid SubirArchivo(IFormFile Archivo)
        {
            ValidarTamano(Archivo);
            string archivo_nombre = Archivo.FileName;
            string nombre_aleatorio = System.IO.Path.GetRandomFileName();
            string extension = Path.GetExtension(archivo_nombre);
            if (!Formatos.Contains(extension.Replace(".", ""))) {
                throw new Exception("Extensión inválida");
            }
            ValidarFormato(archivo_nombre);
            string nombre_aleatorio_ext = $"{nombre_aleatorio}{extension}";
            string ruta_relativa = Path.Combine(this._appSettings.RutaArchivos, nombre_aleatorio_ext);
            string carpeta_absoluta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                _appSettings.RutaArchivos);
            string ruta_absoluta = Path.Combine(carpeta_absoluta, nombre_aleatorio_ext);

            if (!System.IO.Directory.Exists(carpeta_absoluta))
                System.IO.Directory.CreateDirectory(carpeta_absoluta);


            using (var fileStream = new FileStream(ruta_absoluta, FileMode.Create))
            {
                Archivo.CopyTo(fileStream);
            }
            Guid guid = Guid.NewGuid();
            ArchivoTemporal archivo_temporal = new ArchivoTemporal();
            archivo_temporal.guid = guid;
            archivo_temporal.Nombre = archivo_nombre;
            archivo_temporal.Extension = extension;
            archivo_temporal.RutaRelativa = ruta_relativa;
            archivo_temporal.RutaAbsoluta = ruta_absoluta;
            archivo_temporal.FechaCreacion = DateTime.Now;
            InsertarArchivoTmp(archivo_temporal);
            return guid;
        }

        private List<ArchivoTemporal> BuscarAntiguos()
        {
            var query = from x in this.SqliteContext.ArchivosTemporales
                        where x.FechaCreacion <= DateTime.Now.AddHours(-1)
                        select x;
            List<ArchivoTemporal> archivos_temporales = query.ToList<ArchivoTemporal>();
            return archivos_temporales;
        }

        public ArchivoTemporal BuscarArchivoTemporal(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                if (!SqliteContext.ArchivosTemporales.Any(o => o.guid == guid))
                {
                    throw (new KeyNotFoundException("Recurso no encontrado"));
                }
                var query = from x in this.SqliteContext.ArchivosTemporales
                            where x.guid == guid
                            select x;
                ArchivoTemporal archivo_temporal = query.FirstOrDefault<ArchivoTemporal>();
                return archivo_temporal;
            }
            else
            {
                throw (new Exception("El archivo no existe"));
            }
        }

        public bool ExisteArchivo(Guid guid)
        {
            ArchivoTemporal archivo_temporal = BuscarArchivoTemporal(guid);
            string ruta_absoluta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, archivo_temporal.RutaRelativa);
            return File.Exists(ruta_absoluta);

        }

        private void InsertarArchivoTmp(ArchivoTemporal archivo_temporal)
        {
            SqliteContext.ArchivosTemporales.Add(archivo_temporal);
            SqliteContext.SaveChanges();
        }
        public string ObtenerRuta(string uuid)
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _appSettings.RutaArchivos);
            return $"{ruta}/${uuid}";
        }

        private void ValidarTamano(IFormFile Archivo)
        {
            if (Archivo == null)
            {
                throw new Exception("Archivo no se recibió");
            }
            if (Archivo.Length == 0)
            {
                throw (new Exception("El tamaño del archivo es inválido, largo = 0"));
            }
            if (Archivo.Length > tamanoMaximo)
            {
                throw (new Exception($"El tamaño del archivo es inválido, sobrepasa el máximo de {(tamanoMaximo / mb).ToString()} MBs"));
            }
        }
        private Boolean ValidarFormato(string NombreArchivo)
        {
            //throw
            return true;
        }

        public async Task<string> GetGoogleDriveMiniatura(string id ) {
            gdrive gd = new gdrive(_appSettings);
            return await gd.GetMiniatura(id);
        }

        public async Task<Stream> GetGoogleDriveImagen(string id)
        {
            gdrive gd = new gdrive(_appSettings);
            return await gd.GetCompleto(id);
        }



        public async Task<ArchivoBdModel> SubirGoogleDrive(ArchivoTemporal archivo_temporal)
        {
            string ruta = archivo_temporal.RutaAbsoluta;
            string link = "";
            if (File.Exists(ruta))
            {
                string mime =  MimeTypes.Core.MimeTypeMap.GetMimeType(archivo_temporal.Extension);
                gdrive gd = new gdrive(_appSettings);
                long tamano = 0;
                using (var stream = new System.IO.FileStream( ruta, System.IO.FileMode.Open)) {
                    link = await gd.SubirArchivo(stream, archivo_temporal.Nombre, mime);
                    tamano = stream.Length;
                }
                ArchivoBdModel archivo = new ArchivoBdModel()
                {
                    IdArchivo = 0,
                    Ruta = link,
                    Nombre = archivo_temporal.Nombre,
                    Extension = archivo_temporal.Extension,
                    Tamano = tamano,
                    EnGoogleDrive = true
                };
                RespuestaInsercion res = _db.ejecutarSp<RespuestaInsercion>("Utilidades.SpArchivosACT", archivo);
                archivo.IdArchivo = res.Id;
                return archivo;
            }
            else
            {
                throw (new Exception($@"El archivo con Id: {archivo_temporal.guid} no existe, 
                tal ves ha pasado demasiado tiempo desde su creación, verifica"));
            }
           
           //EliminarArchivo(archivo_temporal.guid);
            EliminarAntiguos();
        }

        public ArchivoBdModel GetArchivoBd(int IdAvatar)
        {
            ArchivoBdModel res = _db.consultarSp<ArchivoBdModel>("Seguridad.SpUsuarioAvatarCON", new
            {
                IdAvatar
            }).FirstOrDefault();
            if (res == null) {
                throw new KeyNotFoundException();
            }
            return res;
        }

        public byte[] StreamToByteArray(Stream input)
        {
            byte[] total_stream= new byte[0];
            byte[] stream_array = new byte[0];
            // Setup whatever read size you want (small here for testing)
            byte[] buffer = new byte[32];// * 1024];
            int read = 0;

            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream_array = new byte[total_stream.Length + read];
                total_stream.CopyTo(stream_array, 0);
                Array.Copy(buffer, 0, stream_array, total_stream.Length, read);
                total_stream = stream_array;
            }
            return total_stream;
        }

        public void AgregarFormato(string formato)
        {
            this.Formatos.Add(formato);
        }
    }
}
