using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AspCoreTools.Email
{
    public static class EmailClient
    {
        public static async Task SendEmail(string subject, string text, string recipient)
        {
            const string sender = "krewfindr@gmail.com";
            const string password = "wearedakrew"; //Need to move this somewhere else probably
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(sender, password)
            };

            var message = new MailMessage(sender, recipient, subject, text) {IsBodyHtml = true};

            await client.SendMailAsync(message);
        }
    }
}