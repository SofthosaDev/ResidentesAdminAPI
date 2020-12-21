using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using EvaluadorFinancieraWS.Services.Utilidades;
using Microsoft.AspNetCore.Mvc;
using TuAdelanto.Models;

namespace TuAdelanto.Controllers.Documentos
{
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
            _base = _base;
        }

        [HttpPost("{IdUsuario}")]
        public IActionResult SubirDocumentos([FromBody] List<DocumentosModel> lista_documentos, [FromRoute] int IdUsuario) {
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


                if (!_archivos_service.ExisteArchivo(Guid.Parse(ine_frontal.Guid))) {
                    throw new KeyNotFoundException("INE frontal no se encontró");
                }
                if (!_archivos_service.ExisteArchivo(Guid.Parse(ine_reverso.Guid))) {
                    throw new KeyNotFoundException("INE reverso no se encontró");
                }
                if (!_archivos_service.ExisteArchivo(Guid.Parse(ine_selfie.Guid))) {
                    throw new KeyNotFoundException("INE selfie no se encontró");
                }

                string InePosteriorLink = _archivos_service.SubirGoogleDrive(Guid.Parse(ine_frontal.Guid));
                string IneReversoLink =_archivos_service.SubirGoogleDrive(Guid.Parse(ine_reverso.Guid));
                string IneSelfieLink = _archivos_service.SubirGoogleDrive(Guid.Parse(ine_selfie.Guid));

                _base.ejecutarSp("SpDocumentosALT", new
                {
                    InePosteriorLink,
                    IneReversoLink,
                    IneSelfieLink,
                    IdUsuario
                });
                return Ok();


            }
            catch (Exception er) {
                return BadRequest(er);
            }
        }

    }
}