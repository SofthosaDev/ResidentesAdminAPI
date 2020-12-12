using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TuAdelanto.Services;
using TuAdelanto.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System;
using TuAdelanto.Helpers;
using TuAdelanto.Classes;
using TuAdelanto.Services.Utilidades;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;

namespace TuAdelanto.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
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
            RespuestaInsercionBDModel res = _database.ejecutarSp<RespuestaInsercionBDModel>(
                "SpPerfilACT", 
                perfil);
            if (res.Exito) {
                perfil.IdPerfil = res.Id;
                return Ok(perfil);
            }
            return BadRequest(res);
        }

        [HttpPut("{IdPerfil}")]
        public IActionResult Actualizar([FromRoute] int IdPerfil, Perfil perfil)
        {
            perfil.IdPerfil = IdPerfil;
            RespuestaActualizacionBDModel res = _database.ejecutarSp<RespuestaActualizacionBDModel>(
               "SpPerfilACT",
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
