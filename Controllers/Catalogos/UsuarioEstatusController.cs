using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WsAdminResidentes.Models.Catalogos
{
    [Route("Catalogos/[controller]")]
    [ApiController]
    public class UsuarioEstatusController : ControllerBase
    {
        private readonly IBaseDatosService _db;

        public UsuarioEstatusController(IBaseDatosService _db) {
            this._db = _db;
        }
        
        [HttpGet]
        public  List<UsuarioEstatus> Consultar() {
            return _db.consultarSp<UsuarioEstatus>("Seguridad.SpUsuarioEstatusCON", new { });
        }
    }
}
