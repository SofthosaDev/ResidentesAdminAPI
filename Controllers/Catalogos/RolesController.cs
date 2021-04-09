using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WsAdminResidentes.Models.Catalogos;

namespace WsAdminResidentes.Controllers.Catalogos
{
    [ApiController]
    [Authorize]
    [Route("Catalogos/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IBaseDatosService _db;
        public RolesController(IBaseDatosService _db) {
            this._db = _db;
        }

        [HttpGet]
        public List<Rol>  Consultar(){
            List<Rol> lista = _db.consultarSp<Rol>("Seguridad.SpRolCON", new { });
            return lista;
        }
    }
}
