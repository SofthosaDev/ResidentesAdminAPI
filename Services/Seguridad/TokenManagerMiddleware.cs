using System.Net;
using System.Threading.Tasks;
using WsAdminResidentes.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WsAdminResidentes.Services.Seguridad
{

    public class TokenManagerMiddleware : IMiddleware
    {
        private readonly UsuariosService _usuario_service;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenManagerMiddleware(
            IUsuarioService _u,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _usuario_service = _u as UsuariosService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string auth = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];
            if (auth == null)
            {
                await next(context);
                return;
            }
            auth = auth.Replace("Bearer", "");
            auth = auth.Trim();
            if (await this._usuario_service.EsActivoToken(auth))
            {
                await next(context);
                return;
            }
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }

    public static class TokenManagerMiddlewareExtension
    {
        public static IApplicationBuilder UseTokenManager(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenManagerMiddleware>();
        }
    }
}