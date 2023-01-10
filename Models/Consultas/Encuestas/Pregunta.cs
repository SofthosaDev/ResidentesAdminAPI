using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WsAdminResidentes.Models.Consultas.Encuestas
{
    public class PreguntaConsulta
    {
        public int IdPregunta { get; set; }
        [MaxLength(100), Required]
        public string Titulo { get; set; }
        [Required]
        public int EncuestaId { get; set; }
        public int TipoPreguntaId { get; set; }
        public List<string> Opciones { get; set; }
    }
}