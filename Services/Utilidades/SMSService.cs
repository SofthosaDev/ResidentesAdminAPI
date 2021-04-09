using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WsAdminResidentes.Services
{

    public interface ISMSService
    {
        public Task<object> Enviar(string celular, string titulo, string cuerpo);
    };

    public class SMSService: ISMSService
    {
        /// <summary>
        /// TODO:
        /// Enviar SMS mediante API
        /// Guardar LOG, con json enviado y respuesta
        /// </summary>
        /// <param name="celular"></param>
        /// <param name="titulo"></param>
        /// <param name="cuerpo"></param>
        /// <returns></returns>
        public async Task<object> Enviar(string celular, string titulo, string cuerpo) {

            return new { };
        }
    }
}
