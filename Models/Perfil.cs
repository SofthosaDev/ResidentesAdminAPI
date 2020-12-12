using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuAdelanto.Models
{
    public class Perfil
    {
        public int IdPerfil { get; set; } = 0;
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int AvatarId { get; set; } = 1;
        public int GeneroId { get; set; }
        public int UsuarioId { get; set; } = 0;

    }
}
