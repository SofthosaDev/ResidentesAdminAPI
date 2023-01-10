using WsAdminResidentes.Classes;
using WsAdminResidentes.Models;
using WsAdminResidentes.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using WsAdminResidentes.Models.RespuestasBd;

namespace EvaluadorFinancieraWS.Services.Cobranza.Utilidades
{
    public interface IBaseDatosService
    {
        public List<T> consultarSp<T>(string sp, object parametros) where T : class, new();
        public DataTable consultarSp(string sp, object parametros);
        public T ejecutarSp<T>(string sp, object parametros) where T : class, new();
        public RespuestaBase ejecutarSp(string sp, object parametros);
        public int GetUsuarioFromSession();
    };
    public class BaseDatosService : IBaseDatosService
    {
        private readonly AppSettings appSettings;
        public readonly IHttpContextAccessor _httpContextAccessor;
        public BaseDatosService(
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor
        )
        {
            this.appSettings = appSettings.Value as AppSettings;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<T> consultarSp<T>(string sp, object parametros) where T : class, new()
        {
            int Id_Usuario = 0;
            if (_httpContextAccessor != null)
            {
                string val = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (val != null)
                {
                    Id_Usuario = int.Parse(val);
                }

            }
            int Id_Perfil = 0;
            if (_httpContextAccessor != null)
            {
                string val = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)?.Value;
                if (val != null)
                {
                    Id_Perfil = int.Parse(val);
                }

            }
            List<T> lista;
            DataBase db = new DataBase(appSettings);
            DataSet ds = db.ejecutarSp(sp, parametros, Id_Usuario, Id_Perfil);
            lista = ds.Tables[0].ToList<T>();
            //ds.Dispose();
            //db.Dispose();
            return lista;

        }
        public int GetUsuarioFromSession() {
            int IdUsuario = 0;
            if (_httpContextAccessor != null)
            {
                string val = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (val != null)
                {
                    IdUsuario = int.Parse(val);
                }

            }
            return IdUsuario;
        }
        public T ejecutarSp<T>(string sp, object parametros) where T : class, new()
        {
            int Id_Usuario = 0;
            if (_httpContextAccessor != null)
            {
                string val = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (val != null)
                {
                    Id_Usuario = int.Parse(val);
                }

            }
            int Id_Perfil = 0;
            if (_httpContextAccessor != null)
            {
                string val = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)?.Value;
                if (val != null)
                {
                    Id_Perfil = int.Parse(val);
                }

            }


            List<T> lista;
            DataBase db = new DataBase(appSettings);
            DataSet ds = db.ejecutarSp(sp, parametros, Id_Usuario, Id_Perfil);
            lista = ds.Tables[0].ToList<T>();
            //ds.Dispose();
            //db.Dispose();
            return lista.First();

        }

        public RespuestaBase ejecutarSp(string sp, object parametros)
        {
            return ejecutarSp<RespuestaBase>(sp, parametros);
        }

        public DataTable consultarSp(string sp, object parametros)
        {
            int Id_Usuario = 0;
            if (_httpContextAccessor != null)
            {
                string val = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (val != null)
                {
                    Id_Usuario = int.Parse(val);
                }

            }
            int Id_Perfil = 0;
            if (_httpContextAccessor != null)
            {
                string val = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.PrimarySid)?.Value;
                if (val != null)
                {
                    Id_Perfil = int.Parse(val);
                }

            }

            DataBase db = new DataBase(appSettings);
            DataSet ds = db.ejecutarSp(sp, parametros, Id_Usuario, Id_Perfil);
            return ds.Tables[0];
        }
    }
}