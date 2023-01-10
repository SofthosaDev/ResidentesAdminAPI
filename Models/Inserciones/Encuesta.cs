using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WsAdminResidentes.Models.Inserciones.Encuestas
{
    public class EncuestaInsercion
    {
        public int IdEncuesta { get; set; }
        [MaxLength(150)]
        public string Titulo { get; set; }
        public bool EsVisible { get; set; }
        public bool EsFinalizada { get; set; }
        public bool EsNivelResidencial { get; set; }
        public int ResidencialId { get; set; }
        public List<int> ResidenteIds { get; set; }
    }

    public class PreguntaInsercion {
        public int IdPregunta { get; set; }
        public string Titulo { get; set; }
        public int EncuestaId { get; set; }
        public int TipoPreguntaId { get; set; }
        public List<string> Opciones {get;set;}

    }
}
