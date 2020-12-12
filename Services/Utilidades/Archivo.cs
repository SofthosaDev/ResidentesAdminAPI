using TuAdelanto.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using TuAdelanto.Models.Utilidades;
using TuAdelanto.Services.Utilidades;

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
    }
    public class ArchivosService : IArchivosService
    {
        private readonly SqliteContext _dbContext;
        private readonly AppSettings _appSettings;
        public double tamanoMaximo { get; set; }

        private const double mb = 1048576;
        private readonly SqliteContext SqliteContext;
        public List<string> Formatos;

        public ArchivosService(IOptions<AppSettings> appSettings, int tamano_maximo = 50/*MB*/)
        {
            _appSettings = appSettings.Value as AppSettings;
            tamanoMaximo = 50 * mb;
            _dbContext = new SqliteContext(appSettings);
            SqliteContext = new SqliteContext(appSettings);
            Formatos = new List<string>();
            Formatos.Add("png");
            Formatos.Add("jpeg");
            Formatos.Add("jpg");
            Formatos.Add("gif");
        }

        public void EliminarAntiguos()
        {
            List<ArchivoTemporal> archivos_temporales = this.BuscarAntiguos();
            foreach (ArchivoTemporal archivo_temporal in archivos_temporales)
            {
                EliminarArchivo(archivo_temporal.guid);
            }
        }

        public string MoverArchivo(string carpeta_destino, Guid guid)
        {
            string destino_relativo = "";
            if (!System.IO.Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{carpeta_destino}")))
            {
                System.IO.Directory.CreateDirectory(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{carpeta_destino}")
                );
            }
            string temp = System.IO.Path.GetRandomFileName();
            ArchivoTemporal archivo_temporal = this.BuscarArchivoTemporal(guid);
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
                throw (new Exception($@"El archivo con Id: {guid} no existe, 
                tal ves ha pasado demasiado tiempo desde su creación, verifica"));
            }
            this.EliminarAntiguos();
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
        private string ObtenerRuta(string uuid)
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
    }
}
