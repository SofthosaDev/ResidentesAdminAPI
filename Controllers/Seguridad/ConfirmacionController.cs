using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WsAdminResidentes.Models;
using WsAdminResidentes.Services;
using WsAdminResidentes.Models.RespuestasBd;
using WsAdminResidentes.Models.Argumentos;

namespace WsAdminResidentes.Controllers.Usuarios
{
    [Authorize]
    [Route("[controller]")]
    public class ConfirmacionController : Controller
    {
        public readonly IBaseDatosService _base;
        public readonly ISMSService _sms;
        private readonly IUsuarioService _usuario;

        public ConfirmacionController(IBaseDatosService _base, ISMSService _sms, IUsuarioService _usuario) {
            this._base = _base;
            this._sms = _sms;
            this._usuario = _usuario;
        }


        /// <summary>
        /// Método para saber si el usuario tiene pendiente completar su información de perfil
        /// </summary>
        /// <returns></returns>
        [HttpGet("TieneInformacionCompleta")]
        [Authorize(AuthenticationSchemes = "Temporal")]
        public IActionResult TieneInformacionCompleta()
        {
            RespuestaBase res = _base.ejecutarSp("Seguridad.SpPerfilCompletado", new { });
            return Ok(res);
        }

        /// <summary>
        /// Actualiza la informacion del empleado mediante el token temporal
        [HttpPost("SetearContrasena")]
        [Authorize(AuthenticationSchemes = "Temporal")]
        public IActionResult SetearContrasena([FromBody] string contrasena)
        {
            if (contrasena == null) {
                throw new ArgumentNullException(nameof(contrasena));
            }
            List<string> errores = _usuario.ValidarContrasena(contrasena);
            if (errores.Count > 0) {
                return Ok(new { 
                        Exito = false,
                        Mensaje ="La contraseña no cumple con los requisitos de seguridad",
                        Errores = errores
                });
            }
            contrasena = _usuario.Encriptar(contrasena);
            RespuestaBase res = _base.ejecutarSp("Seguridad.SpEmpleadoContrasenaACT", new {
                Contrasena = contrasena
            });
            return Ok(res);
        }

        /// <summary>
        /// Genera codigo de confirmación, lo guarda en base de datos y lo envía
        /// por SMS
        /// </summary>
        /// <param name="Celular"></param>
        /// <returns></returns>
        ///
        [AllowAnonymous]
        [HttpPost("GenerarCodigo")]
        public async Task<IActionResult> GenerarCodigo(string Celular) {
            try {
                Guid guid = Guid.NewGuid();
                BigInteger bigInt = new BigInteger(guid.ToByteArray());
                string Codigo = bigInt.ToString().Replace("-",""); ;
                Codigo = Codigo.ToString().Substring(0, 5);

                List<Usuario> lista = _base.consultarSp<Usuario>(
                    "Seguridad.SpUsuarioConsultar",
                    new { Celular });
                Usuario usuario = null;

                if (lista.Count > 0) {
                    usuario = lista.First();
                }

                RespuestaInsercion res = _base.ejecutarSp<RespuestaInsercion>(
                    "Seguridad.SpUsuarioCodigoConfirmacionACT", 
                    new 
                    {
                        Codigo,
                        usuario.IdUsuario
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
                List<Usuario> lista = _base.consultarSp<Usuario>(
                    "Seguridad.SpUsuarioConsultar",
                    new { conf.Celular });
                Usuario usuario = null;
                if (lista.Count > 0)
                {
                    usuario = lista.First();
                }

                if (conf == null)
                    throw new Exception("Requeridos");

                RespuestaBase res = _base.ejecutarSp<RespuestaBase>(
                    "Adelantos.SpUsuarioCodigoConfirmacionCON", new { 
                        conf.Celular,
                        conf.CodigoConfirmacion
                    }
                );
                string token = "";
                if (res.Exito) {
                    //res = _base.ejecutarSp<RespuestaBasica>("Adelantos.SpConfirmarUsuarioPROC", new {
                    //    usuario.IdUsuario
                    //});
                    token = _usuario.GenerarTokenTemporal(conf.Celular);
                }

                return Ok(new {
                    res.Exito,
                    res.Codigo,
                    res.Mensaje,
                    token
                });
            }
            catch (Exception er) {
                return BadRequest(er.Message);
            }
            
        }



    }
}