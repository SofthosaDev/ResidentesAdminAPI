using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuAdelanto.Models;
using TuAdelanto.Services;

namespace TuAdelanto.Controllers.Usuarios
{
    [Authorize]//[AllowAnonymous]
    [Route("[controller]")]
    public class ConfirmacionController : Controller
    {
        public readonly IBaseDatosService _base;
        public readonly ISMSService _sms;

        public ConfirmacionController(IBaseDatosService _base, ISMSService _sms) {
            this._base = _base;
            this._sms = _sms;
        }

        /// <summary>
        /// Verifica si el usuario tiene la información requerida completa, retorna un objeto con la propiedad
        /// Exito = True cuando la información necesaria está completa
        /// Exito = False cuando la información aún está pendiente
        /// </summary>
        /// <param name="IdEmpleado"></param>
        /// <returns></returns>
        [HttpGet("TieneInformacionCompleta/{IdEmpleado}")]
        public IActionResult TieneInformacionCompleta([FromRoute] int IdEmpleado) {
            RespuestaBDModel res = _base.ejecutarSp("Adelantos.SpEmpleadoTieneDatosCompletos", new { IdEmpleado });
            return Ok(res);
        }

        /// <summary>
        /// Primero genera el código de confirmación celular
        /// Despues valida que el empleado tenga sus datos obligatorios
        /// completos, si no retorna error DATOS_EMPLEADO_INCOMPLETOS
        /// Finalmente guarda el código de confirmacion en BD, y lo envía por SMS
        /// </summary>
        /// <param name="Celular"></param>
        /// <returns></returns>
        ///
        [AllowAnonymous]
        [HttpPost("GenerarCodigo")]
        public async Task<IActionResult> GenerarCodigo(string Celular) {
            try {
                Guid guid = Guid.NewGuid();
                string Codigo = guid.ToString().Substring(0, 5);



                List<Empleado> lista = _base.consultarSp<Empleado>(
                    "Seguridad.SpUsuariosValidar",
                    new { Nombre = Celular });
                Empleado empleado = null;
                if (lista.Count > 0) {
                    empleado = lista.First();
                }

                //RespuestaBDModel resEmp = _base.ejecutarSp("Adelantos.SpEmpleadoTieneDatosCompletos",
                //    new { empleado.IdEmpleado });
                //if (!resEmp.Exito) {
                //    resEmp.Codigo = CodigoErrores.DATOS_EMPLEADO_INCOMPLETOS;
                //    return Ok(resEmp);
                //}

                RespuestaActualizacionBDModel res = _base.ejecutarSp<RespuestaActualizacionBDModel>(
                    "Adelantos.SpEmpleadoCodigoConfirmacionACT", 
                    new 
                    {
                        Codigo,
                        empleado.IdEmpleado
                    });
                if (!res.Exito) {
                    return Ok(res);
                }

                if (res.Exito) {
                    await _sms.Enviar(
                        Celular, 
                        "Hola", 
                        $@"Tu codigo de confirmación es: <b>{Codigo}<b>, si tu no lo solicitaste ignoralo"
                    );
                }
                return Ok(res);
            }
            catch (Exception er) {
                return BadRequest(er.Message);
            }
        }
        /// <summary>
        /// El usuario envía el código celular a confirmar, este se compara con el almacenado en la base de datos
        /// en caso de ser correcto, marca el registro del empleado como "confirmado"
        /// </summary>
        /// <param name="conf">Objeto que incluye el Id de empleado y el código a verificar </param>
        /// <returns></returns>
        [HttpPost("ConfirmarCodigo")]
        [AllowAnonymous]
        public IActionResult ConfirmarCodigo([FromBody] ConfirmacionCelular conf) {
            try {
                List<Empleado> lista = _base.consultarSp<Empleado>(
                    "Seguridad.SpUsuariosValidar",
                    new { Nombre = conf.Celular });
                Empleado empleado = null;
                if (lista.Count > 0)
                {
                    empleado = lista.First();
                }

                if (conf == null)
                    throw (new Exception("Requeridos"));
                RespuestaBDModel res = _base.ejecutarSp<RespuestaBDModel>("Adelantos.SpEmpleadoCodigoConfirmacionCON", new { 
                    conf.Celular,
                    conf.CodigoConfirmacion
                });
                if (res.Exito) {
                    res = _base.ejecutarSp<RespuestaBDModel>("Adelantos.SpConfirmarUsuarioPROC", new { 
                        empleado.IdEmpleado,
                        Contrasena = BCrypt.Net.BCrypt.HashPassword(conf.Contrasena)
                });
                }
                return Ok(res);
            }
            catch (Exception er) {
                return BadRequest(er.Message);
            }
            
        }



    }
}