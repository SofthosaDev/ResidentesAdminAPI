using System;
using System.Data;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WsAdminResidentes.Classes;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WsAdminResidentes.Models;
using WsAdminResidentes.Helpers;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using WsAdminResidentes.Services.Utilidades;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using WsAdminResidentes.Models.RespuestasBd;

namespace WsAdminResidentes.Services
{
    public interface IUsuarioService
    {
        string Encriptar(string Contrasena);
        Usuario Authenticate(string Nombre, string Contrasena);
        string GenerarTokenTemporal(string nombre);
        IEnumerable<Usuario> GetAll();

        Usuario Insertar(Usuario usuario);
        Task<RespuestaBase> InhabilitarToken(string Authorization, int Id_Token = 0);
        object RefreshSession(string authorization, string refreshToken);
        Task<bool> EsActivoToken(string token);
        RespuestaToken CrearTokenRecuperacion(string Nombre);

        RespuestaBase CambiarContrasena(string Nombre, string Contrasena);

        List<string> ValidarContrasena(string Contrasena);
        RespuestaBase ValidarToken(string Token);

    }



    public class UsuariosService : IUsuarioService
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpContext httpContext;
        private EMailService _email_service;
        private readonly AppSettings _appSettings;
        private readonly IBaseDatosService _databaseService;
        private List<Usuario> _usuarios = new List<Usuario>
        {
        };
        private readonly ISMSService _sms;
        

        public UsuariosService(
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor h,
            IDistributedCache cache,
            IHttpContextAccessor httpContextAccessor,
            IEmailService email_service,
            IBaseDatosService databaseService,
            ISMSService _sms
        )
        {
            _appSettings = appSettings.Value as AppSettings;
            _email_service = email_service as EMailService;
            httpContext = h.HttpContext;
            _cache = cache;
            _databaseService = databaseService;
            this._sms = _sms;
        }


        public async Task<RespuestaBase> InhabilitarToken(string Token, int Id_Token = 0)
        {
            DataBase con = new DataBase(_appSettings);
            RespuestaBDToken res = _databaseService.ejecutarSp<RespuestaBDToken>("Seguridad.SpUsuarioCerrarSesion", new { Token });
            if (res.Exito)
            {
                await _cache.SetStringAsync($"{res.token}:deactive", "");
            }
            return res;
        }

        public string GenerarTokenTemporal(string Celular) {
            List<Usuario> lista_usuarios = _databaseService.consultarSp<Usuario>("Seguridad.SpUsuarioConsultar", new {Celular});
            Usuario usuario = null;
            if (lista_usuarios.Count > 0)
            {
                usuario = lista_usuarios[0];
            }
            if (usuario == null)
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.CredencialTemporal.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(_appSettings.CredencialTemporal.MinutosExpiracion),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            usuario.Token = tokenHandler.WriteToken(token);
            AgregarToken(usuario.IdUsuario, usuario.Token, "");
            return usuario.Token;
        }

        public Usuario Authenticate(string Correo, string Contrasena)
        {
            //DataBase con = new DataBase(_appSettings);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(Contrasena);
            List<Usuario> lista_usuarios =
                _databaseService.consultarSp<Usuario>("Seguridad.SpAdminUsuarioConsultar", new
                {
                    Correo
                });

            //TODO
            //si se regresa mas de un usuario es porque tiene varios usuarios con perfiles para el administrador
            Usuario usuario = null;
            if (lista_usuarios.Count > 0)//Por ahora usaremos el primero
            {
                usuario = lista_usuarios[0];
            }

            if (usuario == null)
                return null;

            //usuario.EstatusClave = 

            bool verified = BCrypt.Net.BCrypt.Verify(Contrasena, usuario.Password);
            if (!verified)
            {
                return null;
            }


            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioPerfilId.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Correo.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                    new Claim(ClaimTypes.PrimarySid, usuario.UsuarioPerfilId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(_appSettings.HorasExpiracion),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string refreshToken = Guid.NewGuid().ToString();
            usuario.Token = tokenHandler.WriteToken(token);
            usuario.RefreshToken = refreshToken;
            AgregarToken(usuario.UsuarioPerfilId, usuario.Token, usuario.RefreshToken);
            return usuario.WithoutPassword();
        }


        private void AgregarToken(int UsuarioPerfilId, string Token, string RefreshToken,
            string AnteriorToken = "")
        {
            DataBase con = new DataBase(_appSettings);
            int minutos = _appSettings.RefreshTokenMinutosVigencia;
            RespuestaBase res = _databaseService.ejecutarSp<RespuestaBase>("Seguridad.SpUsuarioTokenALT", new
            {
                UsuarioPerfilId,
                Token,
                RefreshToken,
                VigenciaRefreshToken = minutos,
                AnteriorToken,

            });
        }

        public IEnumerable<Usuario> GetAll()
        {
            throw new NotImplementedException();
        }

        public Usuario Insertar(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public object RefreshSession(string Authorization, string RefreshToken)
        {
            DataBase con = new DataBase(_appSettings);
            List<Usuario> lista_usuarios = _databaseService.consultarSp<Usuario>("Seguridad.SpUsuariosRefrescarSesion", new
            {
                Authorization,
                RefreshToken
            });
            Usuario usuario = null;
            if (lista_usuarios.Count > 0)
                usuario = lista_usuarios[0];
            if (usuario == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol.ToString())
                }),
                
                Expires = DateTime.UtcNow.AddHours(_appSettings.HorasExpiracion),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string nuevo_refreshToken = Guid.NewGuid().ToString();
            usuario.Token = tokenHandler.WriteToken(token);
            usuario.RefreshToken = nuevo_refreshToken;
            AgregarToken(usuario.IdUsuario, usuario.Token, nuevo_refreshToken, RefreshToken);
            return usuario.WithoutPassword();
        }

        public async Task<bool> EsActivoToken(String token)
        {
            string valor = await _cache.GetStringAsync($"{token}:deactive");
            return valor == null;
        }

        public RespuestaToken CrearTokenRecuperacion(string Nombre)
        {
            DateTime fecha = DateTime.Now;
            fecha = fecha.AddMinutes(30);
            string token = Guid.NewGuid().ToString();
            RecuperacionContrasenaModel rc = new RecuperacionContrasenaModel()
            {
                Token = token,
                FechaLimite = fecha,
                NombreUsuario = Nombre
            };
            Usuario usuario = _databaseService.ejecutarSp<Usuario>("Seguridad.SpUsuarioConsultar", new { 
                    Nombre
            });

            if (usuario.EstatusClave != "01")
            {
                return new RespuestaToken()
                {
                    Exito = false,
                    Mensaje = "No se puede realizar la acción"
                };
            }

            DataBase con = new DataBase(_appSettings);
            RespuestaToken res = _databaseService.ejecutarSp<RespuestaToken>("Seguridad.SpRecuperacionContrasenasACT", rc);
            res.Token = token;
            return res;
        }

        public RespuestaBase ValidarToken(string Token)
        {
            DataBase con = new DataBase(_appSettings);
            RespuestaBase res = _databaseService.ejecutarSp<RespuestaBase>("Seguridad.SpContrasenaValidarToken", new{ Token });
            return res;
        }

        public RespuestaBase CambiarContrasena(string Token, string Contrasena)
        {
            DataBase con = new DataBase(_appSettings);
            Contrasena = BCrypt.Net.BCrypt.HashPassword(Contrasena);
            RespuestaBase res = _databaseService.ejecutarSp<RespuestaBase>("Seguridad.SpCambiarContrasena", new
            {
                Token,
                Contrasena
            });
            return res;
        }

        public string Encriptar(string Contrasena) {
            Contrasena = BCrypt.Net.BCrypt.HashPassword(Contrasena);
            return Contrasena;
        }
        public List<string> ValidarContrasena(string contrasena)
        {
            List<string> errores = new List<string>();
            /*
                Al menos 1 digito
                AL menos 1 letra minuscula
                Al menos 1 letra mayuscula
                At least 8 characters in length, but no more than 32.
            */
            string r_mayusculas = "[A-Z]";
            string r_minusculas = "[a-z]";
            string r_numero = "[0-9]";

            if (contrasena.Length < 8)
            {
                errores.Add("La contraseña debe tener al menos 8 caracteres de largo");
            }
            if (contrasena.Length > 32)
            {
                errores.Add("La contraseña no debe exceder 32 caracteres de largo");
            }
            if (!Regex.IsMatch(contrasena, r_mayusculas))
            {
                errores.Add("La contraseña debe contener por lo menos una letra Mayuscula");
            }
            if (!Regex.IsMatch(contrasena, r_minusculas))
            {
                errores.Add("La contraseña debe contener por lo menos una letra Minuscula");
            }
            if (!Regex.IsMatch(contrasena, r_numero))
            {
                errores.Add("La contraseña debe contener por lo menos un número");
            }
            return errores;
        }
    }
}