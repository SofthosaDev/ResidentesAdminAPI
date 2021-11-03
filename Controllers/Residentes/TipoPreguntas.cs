using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WsAdminResidentes.Models.Residentes;
using WsAdminResidentes.Models.RespuestasBd;


namespace WsAdminResidentes.Controllers.Catalogos
{
    [Authorize, ApiController]
    [Route("[controller]")]
    public class TipoPreguntasController: Controller
    {
        private IBaseDatosService _database;
        public TipoPreguntasController(
            IBaseDatosService _database
        )
        {
            this._database = _database;
        }

        [HttpGet()]
        public IActionResult ConsultarVarios([FromRoute] int IdEncuesta, int UsuarioId, int Limite = 1, int Pagina = 1)
        {
            DataTable dt = _database.consultarSp(
                "Encuestas.SpTipoPreguntaCON", new {});
            return Ok(dt);
        }
    }
}