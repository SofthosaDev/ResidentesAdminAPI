using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WsAdminResidentes.Models.Residencias;

namespace WsAdminResidentes.Controllers.Residenciales
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ResidencialesController : ControllerBase
    {
        private readonly IBaseDatosService _db;
        public ResidencialesController(IBaseDatosService _db) {
            this._db = _db;
        }
        [HttpGet]
        public List<Residencia> ConsultarTodo() {
           List<Residencia> res = _db.consultarSp<Residencia>("Residencial.SpAdminResidencialesCON", new { });
            return res;
        }

        [HttpGet("{IdResidencial}")]
        public IActionResult ConsultarTodo([FromRoute] int IdResidencial)
        {
            Residencia res = _db.consultarSp<Residencia>("Residencial.SpAdminResidentesCON", new
            {
                IdResidencial
            }).FirstOrDefault();

            if (res == null) {
                return NotFound();
            }
            return Ok(res);
        }

    }
}
