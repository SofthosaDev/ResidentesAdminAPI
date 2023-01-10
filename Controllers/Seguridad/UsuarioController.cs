using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using EvaluadorFinancieraWS.Services.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WsAdminResidentes.Classes;
using WsAdminResidentes.Helpers;
using WsAdminResidentes.Models;
using WsAdminResidentes.Models.RespuestasBd;
using WsAdminResidentes.Models.Utilidades;
using WsAdminResidentes.Services;
using WsAdminResidentes.Services.Plantillas;
using WsAdminResidentes.Services.Utilidades;

namespace WsAdminResidentes.Controllers.Seguridad
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IExcelService _excel_service;
        private readonly IArchivosService _archivos_service;
        public readonly IBaseDatosService _base;

        private readonly IUsuarioService _userService;
        private readonly IBaseDatosService _dataBaseService;
        private readonly IPlantillasService _plantillasService;
        private readonly AppSettings _appSettings;
        private EMailService _email_service;
        private readonly IUsuarioService _usuario;

        public UsuarioController(
            IExcelService _excel_service,
            IArchivosService archivos_service,
            IBaseDatosService _base,
            IUsuarioService _usuario,

            IUsuarioService userService,
            IBaseDatosService dataBaseService,
            IPlantillasService plantillasService,
            IOptions<AppSettings> appSettings,
            IEmailService email_service
        )
        {
            this._excel_service = _excel_service;
            _archivos_service = archivos_service;
            this._base = _base;
            _userService = userService;
            _dataBaseService = dataBaseService;
            _plantillasService = plantillasService;
            _appSettings = appSettings.Value as AppSettings;
            _email_service = email_service as EMailService;
            this._usuario = _usuario;
        }

        [HttpGet("ElementosExcel/{guid}")]
        public List<UsuarioBasico> GetElementosExcel(string guid)
        {
            Guid _guid = Guid.Parse(guid);
            ArchivoTemporal archivo = _archivos_service.BuscarArchivoTemporal(_guid);
            List<UsuarioBasico> lista = _excel_service.ConvertirALista<UsuarioBasico>(archivo.RutaAbsoluta);
            _archivos_service.EliminarArchivo(_guid);
            return lista;
        }


        //[HttpPost("Alta/{IdResidencial}")]
        //public IActionResult AltaIndividual([FromBody] UsuarioBasico usuario)
        //{
        //    try
        //    {
        //        if (empleado == null)    
        //            throw new Exception("Empleado requerido");
        //        RespuestaInsercionBDModel res = _base.ejecutarSp<RespuestaInsercionBDModel>("Adelantos.SpEmpleadoALT",
        //            empleado
        //        );
        //        return Ok(res);
        //    }
        //    catch (Exception er)
        //    {
        //        return BadRequest(er);
        //    }

        //}

        [HttpPost("AltaMasiva/{ResidencialId}")]
        public IActionResult AltaEmpleadoMasivo([FromBody] List<UsuarioAltaMasiva> usuarios, [FromRoute] int ResidencialId)
        {
            try
            {
                string JsonUsuarios = JsonSerializer.Serialize(usuarios);
                RespuestaInsercionMasiva res = _base.ejecutarSp<RespuestaInsercionMasiva>(
                "Seguridad.SpUsuariosMasivoALT", new
                {
                    JsonUsuarios,
                    ResidencialId
                });
                return Ok(res);
            }
            catch (ArgumentNullException er)
            {
                return Ok(new RespuestaInsercionMasiva()
                {
                    Exito = false,
                    Mensaje = er.Message
                });
            }
            catch (Exception er)
            {
                return BadRequest(er);
            }

        }

        [HttpGet("{IdUsuario}")]
        public UsuarioConsulta ConsultarUno([FromRoute] int IdUsuario)
        {
            UsuarioConsulta uno = _dataBaseService.consultarSp<UsuarioConsulta>("Seguridad.SpAdminUsuariosCON", new
            {
                IdUsuario
            }).ToList().First();

            return uno;
        }

        [HttpGet("")]
        public List<UsuarioConsulta> GetAll(
            int IdUsuario = 0,
            string NombreUsuario = "",
            int RolId = 0,
            int EstatusId = 0,
            int Limite = 1,
            int Pagina = 1
            )
        {

            List<UsuarioConsulta> lista = _dataBaseService.consultarSp<UsuarioConsulta>("Seguridad.SpAdminUsuariosCON", new
            {
                IdUsuario,
                NombreUsuario,
                EstatusId,
                RolId,
                Limite,
                Pagina
            }).ToList();

            return lista;

        }

        [HttpGet("Preview/{guid}")]
        public IActionResult Preview(string guid)
        {
            Guid _guid = Guid.Parse(guid);
            ArchivoTemporal archivo = _archivos_service.BuscarArchivoTemporal(_guid);
            byte[] imageArray = System.IO.File.ReadAllBytes(archivo.RutaAbsoluta);
            string base64String = Convert.ToBase64String(imageArray);
            return Ok(new { 
                Exito = 1,
                base64 = base64String
            });
        }

        [HttpGet("Avatar/{id}")]
        public async Task<IActionResult> GetAvatar(int id)
        {
            ArchivoBdModel archivo =   _archivos_service.GetArchivoBd(id);

            if (archivo.EnGoogleDrive)
            {
                Stream str = await _archivos_service.GetGoogleDriveImagen(archivo.Ruta);
                byte[] ar = _archivos_service.StreamToByteArray(str);
                string base64String = Convert.ToBase64String(ar);
                return Ok(new
                {
                    Exito = 1,
                    base64 = base64String
                });
            }
            else {
                return NotFound();
            }



        }

        [HttpPut]
        public async Task<IActionResult> Actualizar(UsuarioActualizacion usuario) { 
        
            try
            {
                if (!string.IsNullOrEmpty(usuario.AvatarGuid)) {
                    if (_archivos_service.ExisteArchivo(Guid.Parse(usuario.AvatarGuid))) {
                        ArchivoTemporal archivo = _archivos_service.BuscarArchivoTemporal(Guid.Parse(usuario.AvatarGuid));
                        ArchivoBdModel archivo_bd = await _archivos_service.SubirGoogleDrive(archivo);
                        usuario.AvatarId = archivo_bd.IdArchivo;
                    }
                }
                RespuestaActualizacion resultado = _dataBaseService.ejecutarSp<RespuestaActualizacion>("Seguridad.SpUsuarioACT ", new {
                    usuario.IdUsuario,
                    usuario.Nombre,
                    usuario.EstatusId,
                    usuario.RolId,
                    usuario.PrimerNombre,
                    usuario.SegundoNombre,
                    usuario.ApellidoPaterno,
                    usuario.ApellidoMaterno,
                    usuario.FechaNacimiento,
                    usuario.AvatarId,
                    usuario.GeneroId,
                    usuario.Correo,
                    usuario.Celular
                });
                return Ok(resultado);
            }
            catch (Exception er)
            {
                return BadRequest(er.Message);
            }

        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Crear(UsuarioActualizacion usuario)
        {
            try
            {
                usuario.IdUsuario = 0;
                if (!string.IsNullOrEmpty(usuario.AvatarGuid))
                {
                    if (_archivos_service.ExisteArchivo(Guid.Parse(usuario.AvatarGuid)))
                    {
                        ArchivoTemporal archivo = _archivos_service.BuscarArchivoTemporal(Guid.Parse(usuario.AvatarGuid));
                        ArchivoBdModel archivo_bd = await _archivos_service.SubirGoogleDrive(archivo);
                        usuario.AvatarId = archivo_bd.IdArchivo;
                    }
                }

                RespuestaActualizacion resultado = _dataBaseService.ejecutarSp<RespuestaActualizacion>("Seguridad.SpUsuarioACT ", new {
                    usuario.IdUsuario,
                    usuario.Nombre,
                    usuario.EstatusId,
                    usuario.RolId,
                    usuario.PrimerNombre,
                    usuario.SegundoNombre,
                    usuario.ApellidoPaterno,
                    usuario.ApellidoMaterno,
                    usuario.FechaNacimiento,
                    usuario.AvatarId,
                    usuario.GeneroId,
                    usuario.Correo,
                    usuario.Celular
                });

                if (resultado.Exito)
                {
                    usuario.IdUsuario = resultado.Id;
                    RespuestaToken res = _usuario.CrearTokenRecuperacion(usuario.Nombre);
                    string cont_coreo = _plantillasService.RemplazarPlantilla(
                            _appSettings.Plantillas.Bienvenido,
                            new
                            {
                                nombre = usuario.Nombre,
                                enlace = $@"{_appSettings.UrlAdmin}/#/cambiar_contrasena?Token={res.Token}"
                            }
                        );
                    _email_service.EnviarCorreo(usuario.Correo, "Reseteo de Contraseña", cont_coreo);
                    return Ok(resultado);
                }
                else {
                    return Ok(resultado);
                }
            }
            catch (Exception er)
            {
                return BadRequest(new { Mensaje = er.Message });
            }

        }

        [HttpGet("Nuevos")]
        public IActionResult ConsultarUsuariosNuevos(
            string NombreCompleto = "",
            string Nombre = "",
            string Celular = "",
            int ResidencialId = 0,
            int Limite = 0,
            int Pagina = 0
            )
        {

            DataTable lista = _dataBaseService.consultarSp("Seguridad.SpUsuariosNuevosCON", new
            {
                NombreCompleto,
                Nombre,
                Celular,
                ResidencialId,
                Limite,
                Pagina
            });

            return Ok(lista);
        }


        [HttpGet("NuevosElementosExcel/{guid}")]
        public List<UsuarioBasico> GetUsuariosNuevosElementosExcel([FromRoute] string guid)
        {
            Guid _guid = Guid.Parse(guid);
            ArchivoTemporal archivo = _archivos_service.BuscarArchivoTemporal(_guid);
            List<UsuarioBasico> lista = _excel_service.ConvertirALista<UsuarioBasico>(archivo.RutaAbsoluta);
            _archivos_service.EliminarArchivo(_guid);
            return lista;
        }

    }
}
