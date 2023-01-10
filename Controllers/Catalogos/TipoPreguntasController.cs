using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WsAdminResidentes.Models.RespuestasBd;


namespace WsAdminResidentes.Controllers.Catalogos
{
    [Authorize, ApiController]
    [Route("Catalogos/[controller]")]
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
        public IActionResult ConsultarVarios()
        {
            DataTable dt = _database.consultarSp(
                "Catalogos.SpTipoPreguntaCON", new {});
            return Ok(dt);
        }
    }
}