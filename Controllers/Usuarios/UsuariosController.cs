using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TuAdelanto.Services;
using TuAdelanto.Models;
using System.Linq;
using TuAdelanto.Classes;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;


namespace TuAdelanto.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private IUsuarioService _userService;

        public UsuariosController(IUsuarioService userService)
        {
            _userService = userService;
        }

        [HttpPost("logout/{Id_Usuario?}")]
        public async Task<IActionResult> Logout([FromHeader] string Authorization,
            [FromRoute] int Id_Usuario = 0
        )
        {
            Authorization = Authorization.Replace("Bearer", "");
            Authorization = Authorization.Trim();
            RespuestaBDModel res = await this._userService.InhabilitarToken(Authorization, Id_Usuario);
            if (res.Exito)
                return Ok("Sesión finalizada");
            return BadRequest(res);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AutenticacionModel modelo)
        {
            Usuario user = _userService.Authenticate(modelo.Correo, modelo.Contrasena);

            if (user == null)
                return BadRequest(new
                {
                    Codigo = CodigoErrores.USUARIO_INVALIDO,
                    Exito = false,
                    Mensaje = "Usuario o contraseña incorrecta"
                });
            if (user.EsNuevo)
            {
                return BadRequest(new
                {
                    Codigo = CodigoErrores.USUARIO_NUEVO,
                    Mensaje = "Usuario Nuevo, Realiza el proceso de registro del usuario"
                });
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("refreshSession")]
        public IActionResult RefreshSession(RefreshTokenModel ref_tok, [FromHeader] string Authorization)
        {
            Authorization = Authorization.Replace("Bearer", "");
            Authorization = Authorization.Trim();
            var user = _userService.RefreshSession(Authorization, ref_tok.refreshToken);
            if (user == null)
                return BadRequest(new { 
                    Codigo = CodigoErrores.REFRESH_TOKEN_INCORRECTO,
                    Exito = false,
                    Mensaje = "Refresh token inactivo o incorrecto" });
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("ResetearContrasena")]
        public IActionResult ResetearContrasena(SolicitudToken s)
        {
            RespuestaBDModel res = this._userService.CrearTokenRecuperacion(s.Correo);
            if (res.Exito)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }

        [AllowAnonymous]
        [HttpPost("ValidarTokenReseteo/{Token}")]
        public IActionResult ValidarToken([FromRoute] string Token)
        {
            RespuestaBDModel res = this._userService.ValidarToken(Token);
            if (res.Exito)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }

        [AllowAnonymous]
        [HttpPost("CambiarContrasena/{Token}")]
        public IActionResult CambiarContrasena([FromRoute] string Token, CambioContrasena c)
        {
            List<string> errores = this._userService.ValidarContrasena(c.Contrasena);
            if (errores.Count > 0)
            {
                return BadRequest(new RespuestaBDModel
                {
                    Exito = false,
                    Mensaje = JsonSerializer.Serialize(errores)
                });
            }
            RespuestaBDModel res = this._userService.CambiarContrasena(Token, c.Contrasena);
            if (res.Exito)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("EstaActivo")]
        public IActionResult EstaActivo()
        {
            return Ok();
        }

    }
}
