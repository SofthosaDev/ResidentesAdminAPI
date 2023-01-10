//using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Data;
//using System.Linq;
//using WsAdminResidentes.Models.Residentes;


//namespace wsappresidentes.Controllers.Residentes
//{
//    [Authorize, ApiController]
//    [Route("[controller]")]
//    public class EncuestasController: Controller
//    {
//        private IBaseDatosService _database;
//        public EncuestasController(
//            IBaseDatosService _database
//        )
//        {
//            this._database = _database;
//        }

       
//        [HttpGet("{IdEncuesta}")]
//        public IActionResult ConsultarVarios([FromRoute] int IdEncuesta = 0, int UsuarioId = 0, int Limite = 1, int Pagina = 1)
//        {
//            DataTable dt = _database.consultarSp(
//                "Encuestas.SpEncuestasDisponibles", new { 
//                    IdEncuesta = IdEncuesta,

//                    UsuarioId,
//                    Limite,
//                    Pagina
//                });
//            return Ok(dt);
//        }

//        //[HttpGet("{IdEncuesta}")]
//        //public IActionResult ConsultaUno([FromRoute] int IdEncuesta, [FromQuery] int UsuarioId = 0)
//        //{
//        //    var uno = _database.consultarSp<EncuestaConsulta>("Encuestas.SpRespuestasCON", 
//        //    new { 
//        //        IdEncuesta,
//        //        UsuarioId
//        //    }).ToList().FirstOrDefault();
            
//        //    if (uno == null)
//        //    {
//        //        return NotFound();
//        //    }
//        //    return Ok(uno);
//        //}

//        [HttpPost]
//        public IActionResult Guardar(Encuesta encuesta)
//        {
//            var res = _database.ejecutarSp("Encuestas.SpEncuestaALT", encuesta);
//            return Ok(res);
//        }

//        [HttpPost("{EncuestaId}/Preguntas/{IdPregunta}")]
//        public IActionResult Guardar(
//            [FromRoute] int EncuestaId, 
//            [FromBody] Pregunta pregunta, 
//            [FromRoute] int IdPregunta = 0)
//        {
//            pregunta.IdPregunta = IdPregunta;
//            pregunta.EncuestaId = EncuestaId;

//            var res = _database.ejecutarSp("Encuestas.SpPreguntaALT", pregunta);
//            return Ok(res);
//        }


//    }
//}