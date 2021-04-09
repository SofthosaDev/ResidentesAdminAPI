using System.ComponentModel.DataAnnotations;

namespace WsAdminResidentes.Models
{
    public class AutenticacionModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Contrasena { get; set; }
    }

    public class RefreshTokenModel
    {
        public string refreshToken { get; set; }
    }

}