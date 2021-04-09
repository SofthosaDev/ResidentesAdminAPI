using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WsAdminResidentes.Models.RespuestasBd;

namespace WsAdminResidentes.Models.RespuestasBd
{
    public class RespuestaActualizacion : RespuestaBase
    {
        public int Id { get; set; } = 0;
        public bool Encontrado { get; set; }
    }
}
