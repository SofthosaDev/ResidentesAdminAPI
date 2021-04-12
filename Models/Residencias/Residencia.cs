using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WsAdminResidentes.Models.Residencias
{
    public class Residencia
    {
        public int IdResidencial { get; set; }
        public string Nombre { get; set; }
        public int UbicacionAproximadaId { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public int MunicipioId { get; set; }
        public int EstadoId { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }

    }
}
