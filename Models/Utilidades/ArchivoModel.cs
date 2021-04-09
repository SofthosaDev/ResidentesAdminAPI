using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WsAdminResidentes.Models.Utilidades
{
    public class ArchivoModel
    {
        public string Nombre { get; set; }
        public IFormFile Archivo { get; set; }
    }

    public class ArchivoBdModel{
        public int IdArchivo{ get;set;} = 0;
        public string Ruta { get; set; }
		public bool EnGoogleDrive { get; set; }
		public string Extension { get; set; }
		public long Tamano { get; set; }
		public string Nombre { get; set; }
    }

    public class DocumentoModel: ArchivoBdModel { 
        public string ClaveDocumento { get; set; }
    }

    public class DocumentoBdModel {
        public int IdUsuarioDocumento { get;set; }
        public string ResultadoClave { get; set; }
        public int ResultadoId { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Comentarios { get; set; }
        public int TipoDocumentoId { get; set; }
        public string TipoDocumentoClave { get; set; }
        public string Extension { get; set; }

        public string Ruta { get; set; }
    }
}
