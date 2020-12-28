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
        //REGISTRO
        /*
         * (de lo de abajo sale)
         * Proceso para generar, guardar y enviar codigo por SMS
         * 
         * 
            El empleado "Clickea" en la opción "usuario nuevo"  
                * El empleado escribe 2 veces su número         X
                * Si el usuario cuenta con el beneficio     
                    * Si No cuenta con usuario
                        * Le llega un codigo de verificacion                    ---- EMPLEADO CONFIRMADO
                        * Crea su contraseña
                        * Si el usuario tiene info precargada
                            * La verifica
                        * No tiene info precargada
                            * La captura
                        * Se crea el usuario
                     * Si Cuenta con usuario
                        * "El número celular ya está en uso"     
                * Si no cuenta con el beneficio
                    * FIN
    

            1.- Empleado completa sus datos (Si es necesario)
            2.- Empleado Confirma su número de celular
            3.- El usuario genera su contraseña, se activa
            4.- cARGAR DOCUMENTOS en este paso o dar opción de omitir, 
                los documentos serán obligarios despues de cierta cantidad de adelantos de nómina

            
        */

        public ConfirmacionController(IBaseDatosService _base, ISMSService _sms) {
            this._base = _base;
            this._sms = _sms;
        }

        [HttpPost("GenerarCodigo")]
        public async Task<IActionResult> GenerarCodigo(string Celular) {
            try {
                Guid guid = Guid.NewGuid();
                string Codigo = guid.ToString().Substring(0, 5);

                Empleado empleado = _base.ejecutarSp<Empleado>(
                    "Seguridad.SpUsuariosValidar", 
                    new{ Nombre = Celular });

                
                RespuestaActualizacionBDModel res = _base.ejecutarSp<RespuestaActualizacionBDModel>(
                    "Adelantos.SpEmpleadoCodigoConfirmacionACT", 
                    new 
                    {
                        Codigo,
                        IdEmpleado = empleado.IdEmpleado
                    });
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

        [HttpPost("ConfirmarCodigo")]
        public IActionResult ConfirmarCodigo([FromBody] ConfirmacionCelular conf) {
            RespuestaBDModel res = _base.ejecutarSp<RespuestaBDModel>("Adelantos.SpEmpleadoCodigoConfirmacionCON",conf);
            return Ok(res);
        }



    }
}