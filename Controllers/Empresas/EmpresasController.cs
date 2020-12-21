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

namespace TuAdelanto.Controllers.Empresas
{
    [AllowAnonymous]//[Authorize]
    [Route("[controller]")]
    public class EmpresasController : Controller
    {
        private readonly IExcelService _excel_service;
        private readonly IArchivosService _archivos_service;
        public readonly IBaseDatosService _base;

        public EmpresasController(IExcelService _excel_service, IArchivosService _archivos_service, IBaseDatosService _base) 
        {
            this._excel_service = _excel_service;
            this._archivos_service = _archivos_service;
            this._base = _base;
        }

        [HttpPost("")]
        public IActionResult AltaEmpresa([FromBody] Empresa empresa) {
            try
            {
                RespuestaInsercionBDModel res = _base.ejecutarSp<RespuestaInsercionBDModel>("Adelantos.SpEmpresaALT", empresa);
                return Ok(res);
            }
            catch (Exception er) {
                return BadRequest(er.Message);
            }
        }

        [HttpPost("Masiva")]
        public IActionResult AltaEmpresaMasiva([FromBody] List<Empleado> empleados)
        {

            try
            {
                string jsonEmpleados = JsonSerializer.Serialize(empleados);
                RespuestaInsercionMasivaBDModel res = _base.ejecutarSp<RespuestaInsercionMasivaBDModel>(
                "Adelantos.SpEmpresaMasivaALT", new
                {
                    jsonEmpleados
                });
                return Ok(res);
            }
            catch (Exception er)
            {
                return BadRequest(er);
            }
        }

        [HttpGet("ElementosExcel/{guid}")]
        public List<Empresa> GetElementosExcel(string guid)
        {
            Guid _guid = Guid.Parse(guid);
            ArchivoTemporal archivo = this._archivos_service.BuscarArchivoTemporal(_guid);
            List<Empresa> lista = this._excel_service.ConvertirALista<Empresa>(archivo.RutaAbsoluta);
            return lista;
        }

    }
}