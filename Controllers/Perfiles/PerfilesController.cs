using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WsAdminResidentes.Services;
using WsAdminResidentes.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System;
using WsAdminResidentes.Helpers;
using WsAdminResidentes.Classes;
using WsAdminResidentes.Services.Utilidades;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using WsAdminResidentes.Models.RespuestasBd;

namespace WsAdminResidentes.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PerfilesController : ControllerBase
    {
        private IBaseDatosService _database;

        public PerfilesController(
            IBaseDatosService _database
        )
        {
            this._database = _database;
        }

        [HttpGet("{IdPerfil}")]
        public IActionResult ConsultaUno([FromRoute] int IdPerfil)
        {
            List<Perfil> lista = _database.consultarSp<Perfil>(
                "Seguridad.SpPerfilCONs", new { IdPerfil }).ToList<Perfil>();
            return Ok(lista);
        }

        [HttpPost("")]
        public IActionResult Guardar(Perfil perfil) {
            RespuestaInsercion res = _database.ejecutarSp<RespuestaInsercion>(
                "SpPerfilACT", 
                perfil);
            if (res.Exito) {
                perfil.IdPerfil = res.Id;
                return Ok(perfil);
            }
            return BadRequest(res);
        }

        [HttpPut("{IdPerfil}")]
        [Authorize(AuthenticationSchemes = "Temporal")]
        public IActionResult Actualizar([FromRoute] int IdPerfil, Perfil perfil)
        {
            perfil.IdPerfil = IdPerfil;
            RespuestaActualizacion res = _database.ejecutarSp<RespuestaActualizacion>(
               "Seguridad.SpPerfilACT",
               perfil);

            if (!res.Encontrado)
                return NotFound();
            if (res.Exito)
            {
                return Ok(perfil);
            }
            return BadRequest(res);
        }

    }
}
