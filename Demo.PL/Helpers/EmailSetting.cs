using Demo.DAL.Models;
using System.Net;
using System.Net.Mail;

namespace Demo.PL.Helpers
{
    public class EmailSetting
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true; // Enable SSL/TLS encryption
            client.Credentials = new NetworkCredential("2ndrogirgis@gmail.com", "2804200155aa");
            client.Send("2ndrogirgis@gmail.com", email.Recipients, email.Subject, email.Body);
        }


    }
}
