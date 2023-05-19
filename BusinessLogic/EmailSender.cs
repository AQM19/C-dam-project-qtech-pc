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

        public static void SendVerificationEmail(string dest, string confirmationCode)
        {
            MailAddress to = new MailAddress(dest);
            MailAddress from = new MailAddress(_remitent);

            MailMessage email = new MailMessage(from, to);
            email.Subject = "Testing out email sending";
            email.Body = confirmationCode;

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
            catch (SmtpException e)
            {
                throw;
            }
            finally
            {
                smtp.Dispose();
            }
        }

    }
}
