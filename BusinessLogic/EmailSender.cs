using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mime;

namespace BusinessLogic
{
    public class EmailSender
    {
        private static string _remitent = ConfigurationManager.AppSettings["remitent"].ToString();
        private static string _password = ConfigurationManager.AppSettings["passremitent"].ToString();

        public static void SendVerificationEmail(string dest, string confirmationCode)
        {
            MailAddress to = new MailAddress(dest);
            MailAddress from = new MailAddress(_remitent);

            MailMessage email = new MailMessage(from, to);
            email.Subject = "Testing out email sending";

            string htmlBody = GetVerificationEmailBody(confirmationCode);

            email.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html));

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 25;
            smtp.Credentials = new NetworkCredential(_remitent, _password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;

            try
            {
                smtp.Send(email);
            }
            catch (SmtpException)
            {
                throw;
            }
            finally
            {
                smtp.Dispose();
            }
        }

        private static string GetVerificationEmailBody(string confirmationCode)
        {
            string body = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f8f9fa;
            }}
            .email-container {{
                max-width: 600px;
                margin: auto;
                padding: 20px;
                background-color: white;
                border-radius: 5px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.15);
            }}
            .email-header {{
                color: #007bff;
                text-align: center;
                font-size: 30px;
                border-bottom: 1px solid #ccc;
                padding-bottom: 10px;
            }}
            .email-body {{
                margin-top: 20px;
                font-size: 15px;
                line-height: 1.5;
                color: #333;
            }}
            .confirmation-code {{
                text-align: center;
                margin: 20px 0;
                font-size: 20px;
                color: #007bff;
                font-weight: bold;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <div class='email-header'>
                ¡Gracias por registrarte en AutoTerra!
            </div>
            <div class='email-body'>
                Para completar el proceso de registro, necesitas confirmar tu dirección de correo electrónico. A continuación, encontrarás tu código de verificación:
                <div class='confirmation-code'>
                    {confirmationCode}
                </div>
                Por favor, copia y pega este código en el formulario de verificación para continuar con el registro.
            </div>
        </div>
    </body>
    </html>";

            return body;
        }

    }
}
