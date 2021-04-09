using System;

namespace WsAdminResidentes.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Alias { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }

        public string Token { get; set; }

        public DateTime FechaUltimaSesion { get; set; }

        public string Rol { get; set; } 

        public string RefreshToken { get; set; }

        public string EstatusClave { get; set; }
        public bool EsNuevo { get; set; }

        public string CodigoCelular { get; set; }

        public string Celular { get; set; }

        //public string ClaveRol { get; set; }
        //public string Rol { get; set; }

    }

    public class CambioContrasena
    {
        public string Contrasena { get; set; }
    }

    public class SolicitudToken
    {
        public string NombreUsuario { get; set; }
    }

    public class UsuarioConsulta
    {
        public int TotalRegistros { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public int EstatusId { get; set; }
        public string Estatus { get; set; }
        public string EstatusClave { get; set; }
        public DateTime FechaAlta { get; set; }
        public int RolId { get; set; }
        public string Rol { get; set; }
        public string RolClave { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaUltimaSesion { get; set; }
        public bool EsNuevo { get; set; }
        public string Correo { get; set; }
        public string Celular { get; set; }
        public int AvatarId { get; set; }
    }

    public class UsuarioActualizacion {
        public int IdUsuario  {get;set;}
        public string Alias { get; set; }
        public string Nombre { get; set; }
        public int EstatusId { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
        public int UsuarioAccionId { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre {get;set;}
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno {get; set;}
        public DateTime? FechaNacimiento { get; set; } = null;
        public int AvatarId  {get; set;}
        public string AvatarGuid { get; set; }
        public int GeneroId { get; set; }
        public string Correo { get; set; }
        public string Celular { get; set; }
    }

    public class UsuarioAltaMasiva { 
        public string Celular { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
    }
}