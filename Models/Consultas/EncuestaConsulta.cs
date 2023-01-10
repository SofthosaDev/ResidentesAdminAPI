using System;
using System.Collections.Generic;

namespace WsAdminResidentes.Models.Consultas.Encuestas
{
    public class Encuesta
    {
        public int TotalRegistros { get; set; }
        public int IdEncuesta { get; set; }
        public string Titulo { get; set; }
        public bool EsFinalizada { get; set; }
        public bool EsNivelResidencial { get; set; }
        public int ResidencialId { get; set; }
        public List<int> ResidenteIds { get;set; }
        public DateTime FechaAlta { get; set; }
    }
}
