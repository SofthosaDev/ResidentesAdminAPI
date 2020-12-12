using System;

namespace TuAdelanto.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Alias { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }

        public string Token { get; set; }

        public DateTime FechaUltimaSesion { get; set; }

        public string Rol { get; set; } 

        public string RefreshToken { get; set; }

        public string EstatusClave { get; set; }
        public bool EsNuevo { get; set; }

        //public string ClaveRol { get; set; }
        //public string Rol { get; set; }

    }

    public class CambioContrasena
    {
        public string Contrasena { get; set; }
    }

    public class SolicitudToken
    {
        public string Correo { get; set; }
    }
}