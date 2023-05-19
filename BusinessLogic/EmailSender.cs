using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace BusinessLogic
{
    public class EmailSender
    {
        private static string _remitent = ConfigurationManager.AppSettings["remitent"].ToString();
        private static string _password = ConfigurationManager.AppSettings["passremitent"].ToString();
        private static string _clientId = ConfigurationManager.AppSettings["clientId"].ToString();
        private static string _clientSecret = ConfigurationManager.AppSettings["clientSecret"].ToString();

        public static void SendVerificationEmail(string dest, string confirmationCode)
        {
            string affair = "Confirmación de registro.";
            string body = $"Gracias por registrarte en QTech - AutoTerra. Tu código de confirmación es: {confirmationCode}";

            SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com", 587);
            clienteSmtp.EnableSsl = true;
            clienteSmtp.Credentials = new NetworkCredential(_remitent, _password);
            clienteSmtp.UseDefaultCredentials = false;
            clienteSmtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            clienteSmtp.ClientCertificates.Add(new X509Certificate2("ruta_al_archivo_certificado.p12", "contraseña_certificado"));

            MailMessage mensaje = new MailMessage(_remitent, dest, affair, body);

            try
            {
                clienteSmtp.Send(mensaje);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
