using Dapper;
using OTS.Data.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace OTS.Data.Repository
{
    public class SendMailRepositery: ISendMailRepositery
    {
        private readonly ISqlDataAccess _db;

        public SendMailRepositery(ISqlDataAccess db)
        {
            _db = db;
        }

        public async void Send_Mail(string emailid, string subject, string msg)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse("otsordertesting@gmail.com");
                email.To.Add(MailboxAddress.Parse(emailid));
                email.Subject = subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = msg;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("otsordertesting@gmail.com", "wsin qlez cfmy stcu");
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
