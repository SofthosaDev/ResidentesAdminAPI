using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WsAdminResidentes.Controllers.Encuestas
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class EncuestasController : ControllerBase
    {
        private readonly IBaseDatosService _db;
        public EncuestasController(IBaseDatosService _db)
        {
            this._db = _db;
        }
        
        [HttpGet("")]
        public List<Models.Consultas.Encuestas.Encuesta> GetAll(
            string Titulo = "",
            int EstatusId = 0,
            int ResidencialId = 0,
            int Limite = 100,
            int Pagina = 1)
        {
            var lista = _db.consultarSp<Models.Consultas.Encuestas.Encuesta>("Encuesta.SpAdminEncuestasCON", new {
                Titulo,
                EstatusId,
                ResidencialId,
                Limite,
                Pagina
            });
            return lista;
        }

        [HttpGet("{IdEncuesta}")]
        public IActionResult ConsultarUno([FromRoute] int IdEncuesta)
        {
            var lista = _db.consultarSp<Models.Consultas.Encuestas.Encuesta>("Encuesta.SpAdminEncuestasCON", new
            {
                IdEncuesta
            });
            if (!lista.Any()) {
                return NotFound();
            }
            return Ok(lista.FirstOrDefault());
        }

        [HttpPost()]
        public IActionResult InsertarNuevo(Models.Inserciones.Encuestas.EncuestaInsercion modelo)
        {

            if (!ModelState.IsValid) {
                return BadRequest();
            }
            
            var res = _db.ejecutarSp("Encuesta.SpAdminEncuestasACT",new {
                modelo.IdEncuesta,
                modelo.Titulo,
                modelo.EsVisible,
                modelo.EsFinalizada,
                modelo.EsNivelResidencial,
                modelo.ResidencialId,
                ResidenteIds = (modelo.ResidenteIds==null)?"":string.Join(",", modelo.ResidenteIds),
            });
            return Ok(res);
        }

        [HttpPut("{IdEncuesta}")]
        public IActionResult ModificarExistente([FromRoute]int IdEncuesta, Models.Inserciones.Encuestas.EncuestaInsercion modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            modelo.IdEncuesta = IdEncuesta;
            var res = _db.ejecutarSp("Encuesta.SpAdminEncuestasACT", new
            {
                modelo.IdEncuesta,
                modelo.Titulo,
                modelo.EsVisible,
                modelo.EsFinalizada,
                modelo.EsNivelResidencial,
                modelo.ResidencialId,
                ResidenteIds = (modelo.ResidenteIds == null) ? "" : string.Join(",", modelo.ResidenteIds),
            });

            if (res.Exito && res.Id == 0) {
                return NotFound();
            }
            return Ok(res);
        }

        [HttpPost("{IdEncuesta}/Pregunta")]
        public IActionResult CrearPregunta([FromRoute] int IdEncuesta, Models.Inserciones.Encuestas.PreguntaInsercion modelo) {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            modelo.EncuestaId = IdEncuesta;
            var res = _db.ejecutarSp("Encuesta.SpAdminEncuestasPreguntaACT", new { 
                modelo.IdPregunta,
                modelo.Titulo,
                modelo.EncuestaId,
                modelo.TipoPreguntaId,
                Opciones = (modelo.Opciones == null) ? "" : string.Join(",", modelo.Opciones)
            });

            if (res.Exito && res.Id == 0)
            {
                return NotFound();
            }
            return Ok(res);
        }

        [HttpPut("{IdEncuesta}/Pregunta/{IdPregunta}")]
        public IActionResult ActualizarPregunta(
            [FromRoute] int IdEncuesta, 
            [FromRoute] int IdPregunta, 
            Models.Inserciones.Encuestas.PreguntaInsercion modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            modelo.IdPregunta = IdPregunta;
            modelo.EncuestaId = IdEncuesta;
            var res = _db.ejecutarSp("Encuesta.SpAdminEncuestasPreguntaACT", new
            {
                modelo.IdPregunta,
                modelo.Titulo,
                modelo.EncuestaId,
                modelo.TipoPreguntaId,
                Opciones = (modelo.Opciones == null) ? "" : string.Join(",", modelo.Opciones)
            });

            if (res.Exito && res.Id == 0)
            {
                return NotFound();
            }
            return Ok(res);
        }

        [HttpGet("{EncuestaId}/Preguntas")]
        public IActionResult ConsultarTodasPreguntas([FromRoute] int EncuestaId) {
            var lista = _db.consultarSp<Models.Consultas.Encuestas.PreguntaConsulta>("Encuesta.SpAdminEncuestasPreguntaCON", new
            {
                EncuestaId
            });
            return Ok(lista);
        }

        [HttpGet("{EncuestaId}/Preguntas/{PreguntaId}")]
        public IActionResult ConsultarUnaPregunta([FromRoute] int EncuestaId, [FromRoute] int PreguntaId)
        {
            var lista = _db.consultarSp<Models.Consultas.Encuestas.PreguntaConsulta>("Encuesta.SpAdminEncuestasPreguntaCON", new
            {
                EncuestaId,
                PreguntaId
            });
            if (!lista.Any())
            {
                return NotFound();
            }
            return Ok(lista.FirstOrDefault());
        }

        [HttpDelete("{IdEncuesta}/Pregunta/{IdPregunta}")]
        public IActionResult EliminarPregunta(
           [FromRoute] int EncuestaId,
           [FromRoute] int IdPregunta)
        {
         
            var res = _db.ejecutarSp("Encuesta.SpAdminEncuestasPreguntaELI", new{ EncuestaId, IdPregunta});

            if (res.Exito && res.Id == 0)
            {
                return NotFound();
            }
            return Ok(res);
        }

    }
}
