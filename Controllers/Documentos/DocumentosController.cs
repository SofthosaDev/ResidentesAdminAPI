using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using EvaluadorFinancieraWS.Services.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerrosApp.Classes;
using TuAdelanto.Models;
using TuAdelanto.Models.Utilidades;
using static System.Net.WebRequestMethods;

namespace TuAdelanto.Controllers.Documentos
{
    [Authorize]//[AllowAnonymous]
    [Route("[controller]")]
    [Authorize(Policy = "AccesoTemporal")]
    public class DocumentosController : Controller
    {
        private readonly IArchivosService _archivos_service;
        public readonly IBaseDatosService _base;
        public DocumentosController(
           IArchivosService archivos_service,
           IBaseDatosService _base
       )
        {
            _archivos_service = archivos_service;
            this._base = _base;
        }

        [HttpGet("Visualizar/{IdUsuarioDocumento}")]
        public async Task<FileStreamResult> ConsultaDocumento([FromRoute] int IdUsuarioDocumento)
        {
                List<DocumentoBdModel> lista = _base.consultarSp<DocumentoBdModel>("Adelantos.SpUsuarioDocumentosCON", new { IdUsuarioDocumento });
                DocumentoBdModel l = lista.First();
                System.IO.Stream s = await _archivos_service.GetGoogleDriveImagen(l.Ruta);
                string mime = MimeTypes.Core.MimeTypeMap.GetMimeType(l.Extension);
            FileStreamResult fsr = File(s, mime);
            fsr.FileDownloadName = $"{l.TipoDocumentoClave}{l.Extension}";
            return fsr;

        }

        [HttpGet("{UsuarioId}")]
        public async Task<IActionResult> ConsultaDocumentos() {
            try {
                List<DocumentoBdModel> lista = _base.consultarSp<DocumentoBdModel>("Adelantos.SpUsuarioDocumentosCON",new { });
                List<Object> lista_final = new List<Object>();

                foreach (DocumentoBdModel l in lista) {
                    string res = await _archivos_service.GetGoogleDriveMiniatura(l.Ruta);
                    lista_final.Add(new {
                        l.IdUsuarioDocumento,
                        l.ResultadoClave,
                        l.ResultadoId,
                        l.FechaAlta,
                        l.Comentarios,
                        l.TipoDocumentoClave,
                        Miniatura = res
                    });
                }
                
                return Ok(lista_final);
            }
            catch (Exception er) {
                return BadRequest(er);
            }

        }

        [HttpPost("{UsuarioId}")]
        public async Task<IActionResult> SubirDocumentos([FromBody] List<DocumentosModel> lista_documentos, [FromRoute] int UsuarioId) {
            DocumentosModel ine_frontal = lista_documentos.Find(x=> x.Clave=="01");
            DocumentosModel ine_reverso = lista_documentos.Find(x => x.Clave == "02");
            DocumentosModel ine_selfie = lista_documentos.Find(x => x.Clave == "03");

            try {
                if (ine_frontal == null)
                    throw new Exception("INE Frontal requerida");
                if (ine_reverso == null)
                    throw new Exception("INE Reverso requerida");
                if (ine_selfie == null)
                    throw new Exception("INE Selfie requerida");

                List<DocumentoModel> lista_final = new List<DocumentoModel>();
                foreach (DocumentosModel dm in lista_documentos) {

                    ArchivoTemporal temporal = _archivos_service.BuscarArchivoTemporal(Guid.Parse(ine_frontal.Guid));
                    System.IO.FileInfo fi = new System.IO.FileInfo(temporal.RutaAbsoluta);
                    string link = await _archivos_service.SubirGoogleDrive(temporal);
                    lista_final.Add(new DocumentoModel() { 
                        Ruta = link,
                        EnGoogleDrive = true,
                        Extension = temporal.Extension,
                        Tamano = fi.Length,
                        Nombre = temporal.Nombre,
                        ClaveDocumento = dm.Clave
                    });

                }

                RespuestaBDModel res = _base.ejecutarSp("Adelantos.SpUsuarioDocumentosALT", new
                {
                    DocumentosJson = lista_final,
                    UsuarioId
                });
                return Ok(res);


            }
            catch (Exception er) {
                return BadRequest(er);
            }
        }

    }
}