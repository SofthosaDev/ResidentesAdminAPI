using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TuAdelanto.Services;
using TuAdelanto.Models;
using TuAdelanto.Classes;
using System.Net;
using TuAdelanto.Models.Utilidades;
using System.IO;
using TuAdelanto.Services.Utilidades;
using EvaluadorFinancieraWS.Services.Utilidades;

namespace TuAdelanto.Controllers
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
            this._service = service;
        }

        [HttpPost]
        public ActionResult Subir([FromForm] ArchivoModel std)
        {
            try {
                string Nombre = std.Nombre;
                var Archivo = std.Archivo;
                Guid ruta = this._service.SubirArchivo(Archivo);

                return Ok(new
                {
                    Ruta = ruta,
                    Exito = 1,
                    Mensaje = "Archivo Cargado correctamente"
                });
            }
            catch (Exception er) {
                return BadRequest(er);
            }
        }
    }
}