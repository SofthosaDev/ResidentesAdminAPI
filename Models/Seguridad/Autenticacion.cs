using System.ComponentModel.DataAnnotations;

namespace TuAdelanto.Models
{
    public class AutenticacionModel
    {
        [Required]
        public string Correo { get; set; }

        [Required]
        public string Contrasena { get; set; }
    }

    public class RefreshTokenModel
    {
        public string refreshToken { get; set; }
    }

}