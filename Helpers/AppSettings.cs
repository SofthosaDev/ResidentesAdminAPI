﻿namespace WsAdminResidentes.Helpers
{

    public class Plantillas
    {
        public string RecuperacionContrasena { get; set; }
        public string Bienvenido { get; set; }
    }
    public class Correo
    {
        public string host { get; set; }
        public int port { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public bool enableSsl { get; set; }
    }

    public class CredencialTemporal { 
        public string Secret { get; set; }
        public double MinutosExpiracion { get; set; }
    }
    public class AppSettings
    {
        public string Secret { get; set; }
        public string UrlAdmin { get; set; }
        public string CadenaConexion { get; set; }
        public double HorasExpiracion { get; set; }

        public string SP_ACT { get; set; }
        public string SP_CON { get; set; }
        public string SP_BAJ { get; set; }
        public string RutaArchivos { get; set; }

        public string SQLITE_DB { get; set; }
        public int RefreshTokenMinutosVigencia { get; set; } = 0;

        public Correo Correo { get; set; }

        public string GDriveKey { get; set; }

        public CredencialTemporal CredencialTemporal { get; set; }
        public Plantillas Plantillas { get; set; }
    }


}