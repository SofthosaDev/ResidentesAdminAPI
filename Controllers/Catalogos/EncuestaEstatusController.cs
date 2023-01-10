using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WsAdminResidentes.Models.Catalogos;

namespace WsAdminResidentes.Controllers.Catalogos
{
    [ApiController]
    [Authorize]
    [Route("Catalogos/[controller]")]
    public class EncuestaEstatusController : ControllerBase
    {
        private readonly IBaseDatosService _db;
        public EncuestaEstatusController(IBaseDatosService _db)
        {
            this._db = _db;
        }

        [HttpGet]
        public List<EncuestaEstatus> Consultar()
        {
            var res = new List<EncuestaEstatus>();
            res.Add(new EncuestaEstatus() {
                IdEstatus = 0,
                Estatus = "Activa"
            });
            res.Add(new EncuestaEstatus()
            {
                IdEstatus = 1,
                Estatus = "Finalizada"
            });
            return res;
        }
    }
}
