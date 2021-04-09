using System;
using System.Net;
using System.Net.Mail;
using WsAdminResidentes.Classes;
using WsAdminResidentes.Helpers;
using Microsoft.Extensions.Options;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;

namespace WsAdminResidentes.Services.Utilidades
{
    public interface IEmailService
    {
        public void EnviarCorreo(string destinatario, string asunto, string mensaje, bool esHTML = false);
        public void EnviarCorreo(MailMessage mensaje);
    }

    public class EMailService : IEmailService
    {
        private readonly AppSettings appSettings;
        private SmtpClient cliente;
        private readonly IBaseDatosService _databaseService;

        public EMailService(
            IOptions<AppSettings> _appset,
            IBaseDatosService databaseService
        )
        {
            _databaseService = databaseService;
            this.appSettings = _appset.Value as AppSettings;
            cliente = new SmtpClient(this.appSettings.Correo.host, this.appSettings.Correo.port)
            {
                EnableSsl = this.appSettings.Correo.enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(this.appSettings.Correo.user, this.appSettings.Correo.password)
            };

        }
        public void EnviarCorreo(string destinatario, string asunto, string mensaje, bool esHTML = false)
        {
            MailMessage correo = new MailMessage(this.appSettings.Correo.user, destinatario, asunto, mensaje);
            correo.IsBodyHtml = esHTML;
            string error = "";
            bool hubo_error = false;
            try
            {
                cliente.Send(correo);
            }
            catch (Exception ex)
            {
                error = ex.Message.ToString();
                hubo_error = true;
            }
            try
            {
                this.GuardarCorreo(correo, hubo_error, error);
            }
            catch (Exception er)
            {

            }
        }

        public void EnviarCorreo(MailMessage correo)
        {
            string error = "";
            bool hubo_error = false;
            try
            {
                cliente.Send(correo);
            }
            catch (Exception ex)
            {
                error = ex.Message.ToString();
                hubo_error = true;
            }
            try
            {
                this.GuardarCorreo(correo, hubo_error, error);
            }
            catch (Exception er)
            {

            }
        }

        private void GuardarCorreo(MailMessage correo, bool hubo_error, string error)
        {
            DataBase con = new DataBase(this.appSettings);

            _databaseService.ejecutarSp("Seguridad.Correos", new
            {
                Destinatario = correo.To,
                Asunto = correo.Subject,
                Mensaje = correo.Body,
                EsHtml = correo.IsBodyHtml,
                OcurrioError = hubo_error,
                MensajeError = error
            });

        }
    }



}