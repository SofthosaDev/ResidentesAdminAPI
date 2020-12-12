using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using EvaluadorFinancieraWS.Services.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuAdelanto.Models;
using TuAdelanto.Models.Utilidades;
using TuAdelanto.Services.Utilidades;

namespace TuAdelanto.Controllers.Empleados
{
    [Authorize]
    [Route("[controller]")]
    public class EmpleadosController : Controller
    {
        private readonly IExcelService _excel_service;
        private readonly IArchivosService _archivos_service;
        public readonly IBaseDatosService _base;
        public EmpleadosController(
            IExcelService _excel_service,
            IArchivosService archivos_service,
            IBaseDatosService _base
        ) {
            this._excel_service = _excel_service;
            this._archivos_service = archivos_service;
            this._base = _base;
        }

        [HttpGet("ElementosExcel/{guid}")]
        public List<Empleado> GetElementosExcel(string guid)
        {
            Guid _guid = Guid.Parse(guid);
            ArchivoTemporal archivo = this._archivos_service.BuscarArchivoTemporal(_guid);
            List<Empleado> lista = this._excel_service.ConvertirALista<Empleado>(archivo.RutaAbsoluta);
            return lista;
        }

        [HttpPost("Alta")]
        [AllowAnonymous]
        public IActionResult AltaIndividual([FromBody] Empleado empleado) {
            try {
                RespuestaInsercionBDModel res = _base.ejecutarSp<RespuestaInsercionBDModel>("Adelantos.SpEmpleadoALT",
                    empleado
                );
                return Ok(res);
            }
            catch (Exception er) {
                return BadRequest(er);
            }

        }

        [HttpPost("AltaMasiva")]
        public IActionResult AltaEmpleadoMasivo([FromBody] List<Empleado> empleados) {
            
            try {
                string jsonEmpleados = JsonSerializer.Serialize(empleados);
                RespuestaInsercionMasivaBDModel res = _base.ejecutarSp<RespuestaInsercionMasivaBDModel>(
                "Adelantos.SpEmpleadosMasivoALT", new
                {
                    jsonEmpleados
                });
                return Ok(res);
            }
            catch (Exception er) {
                return BadRequest(er);
            }
        }

    }
}