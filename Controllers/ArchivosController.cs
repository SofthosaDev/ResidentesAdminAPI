using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WsAdminResidentes.Services;
using WsAdminResidentes.Models;
using WsAdminResidentes.Classes;
using System.Net;
using WsAdminResidentes.Models.Utilidades;
using System.IO;
using WsAdminResidentes.Services.Utilidades;
using EvaluadorFinancieraWS.Services.Utilidades;
using Microsoft.AspNetCore.Http;

namespace WsAdminResidentes.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ArchivosController : ControllerBase
    {
        private IArchivosService _service;
        public ArchivosController(IArchivosService service)
        {
            _service = service;
            _service.AgregarFormato(Formato.JPG);
            _service.AgregarFormato(Formato.PNG);
            _service.AgregarFormato(Formato.GIF);
        }

        /// <summary>
        /// Sube un archivo a un espacio temporal
        /// </summary>
        /// <param name="std"></param>
        /// <returns>GUID: El GUID Retornado identifica la ubicación del archivo, 
        /// este ID se utiliza como si fuera el archivo en si, una ves recibido por otros 
        /// métodos es "confirmado", almacenado en la nube (google drive) y eliminado
        /// del espacio temporal</returns>
        [HttpPost]
        public ActionResult Subir([FromForm] ArchivoModel std)
        {
            try {
                string Nombre = std.Nombre;
                var Archivo = std.Archivo;
                //_service.AgregarFormato("JPG");
                //_service.AgregarFormato("JPEG");

                Guid ruta = _service.SubirArchivo(Archivo);

                return Ok(new {
                    Ruta = ruta,
                    Exito = 1,
                    Mensaje = "Archivo Cargado correctamente"
                });
            }
            catch (Exception er) {
                return BadRequest(er);
            }
        }

        /// <summary>
        /// Sube archivos tipo Excel
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns>Retorna la ruta donde el archivo estará almacenado</returns>
        [HttpPost("CargarExcel")]
        public ActionResult SubirExcel(IFormFile archivo)
        {
            //RESTRINGIR A EXCEL
            try
            {
                string Nombre = "carga";
                _service.AgregarFormato(Formato.XLSX);
                Guid ruta = _service.SubirArchivo(archivo);

                return Ok(new
                {
                    Ruta = ruta,
                    Exito = 1,
                    Mensaje = "Archivo Cargado correctamente"
                });
            }
            catch (Exception er)
            {
                return BadRequest(er);
            }
        }

        [HttpPost("Subir")]
        public ActionResult Subir(IFormFile avatar)
        {
            //RESTRINGIR A IMAGNES
            try
            {
                string Nombre = "Avatar";
                Guid ruta = _service.SubirArchivo(avatar);

                return Ok(new
                {
                    Ruta = ruta,
                    Exito = 1,
                    Mensaje = "Archivo Cargado correctamente"
                });
            }
            catch (Exception er)
            {
                return BadRequest(er);
            }
        }
    }
}