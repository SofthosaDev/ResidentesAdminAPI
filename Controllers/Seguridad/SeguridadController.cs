using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WsAdminResidentes.Services;
using WsAdminResidentes.Models;
using System.Linq;
using WsAdminResidentes.Classes;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using WsAdminResidentes.Models.RespuestasBd;

namespace WsAdminResidentes.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize]//[AllowAnonymous]
    public class SeguridadController : ControllerBase
    {
        private IUsuarioService _userService;

        public SeguridadController(IUsuarioService userService)
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
            RespuestaBase res = await _userService.InhabilitarToken(Authorization, Id_Usuario);
            if (res.Exito)
                return Ok("Sesión finalizada");
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AutenticacionModel modelo)
        {
            Usuario user = _userService.Authenticate(modelo.Nombre, modelo.Contrasena);
            if (user == null)
            {
                return BadRequest(new
                {
                    Codigo = CodigoErrores.USUARIO_INVALIDO,
                    Exito = false,
                    Mensaje = "Usuario o contraseña incorrecta"
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
            RespuestaBase res = _userService.CrearTokenRecuperacion(s.NombreUsuario);
            return Ok(res);

        }

        [AllowAnonymous]
        [HttpPost("ValidarTokenReseteo/{Token}")]
        public IActionResult ValidarToken([FromRoute] string Token)
        {
            RespuestaBase res = this._userService.ValidarToken(Token);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpPost("CambiarContrasena/{Token}")]
        public IActionResult CambiarContrasena([FromRoute] string Token, CambioContrasena c)
        {
            List<string> errores = this._userService.ValidarContrasena(c.Contrasena);
            if (errores.Count > 0)
            {
                return Ok(new RespuestaBase
                {
                    Exito = false,
                    Mensaje = JsonSerializer.Serialize(errores)
                });
            }
            RespuestaBase res = this._userService.CambiarContrasena(Token, c.Contrasena);
            return Ok(res);
        }

        [HttpGet]
        [Route("EstaActivo")]
        public IActionResult EstaActivo()
        {
            return Ok();
        }

    }
}
