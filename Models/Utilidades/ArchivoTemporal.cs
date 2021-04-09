using System;
using System.ComponentModel.DataAnnotations;

namespace WsAdminResidentes.Models.Utilidades
{
    public class ArchivoTemporal
    {
        [Key]
        public int Id { get; set; }
        public Guid guid { get; set; }
        public string Nombre { get; set; }
        public string Extension { get; set; }
        public string RutaRelativa { get; set; }
        public string RutaAbsoluta { get; set; }

        public DateTime FechaCreacion { get; set; }

        public ArchivoTemporal()
        {
            this.Id = 0;
            this.guid = new Guid();
            this.Nombre = "";
            this.Extension = "";

        }

    }
}