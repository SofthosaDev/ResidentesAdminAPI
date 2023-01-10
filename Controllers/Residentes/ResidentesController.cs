using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WsAdminResidentes.Models.Residencias;

namespace WsAdminResidentes.Controllers.Residentes
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ResidentesController : ControllerBase
    {
        private readonly IBaseDatosService _db;
        public ResidentesController(IBaseDatosService _db) {
            this._db = _db;
        }
        [HttpGet("{IdResidencial}")]
        public IActionResult ConsultarTodo(int IdResidencial) {
           var res = _db.consultarSp("Residencial.SpAdminResidentesCON", new {
               IdResidencial
           });
            return Ok(res);
        }

       

    }
}
