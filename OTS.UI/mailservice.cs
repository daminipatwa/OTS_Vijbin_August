using MailKit.Net.Smtp;

using MailKit.Security;
using MimeKit;



namespace OTS.UI
{
    public class mailservice
    {
        

        public async void send_email(string emailid,string subject,string msg)
        {
            //MailMessage mailMessage = new MailMessage("otsordertesting@gmail.com", emailid, subject, msg);
            //SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            //{
            //    Port = 587,
            //    Credentials = new NetworkCredential("otsordertesting@gmail.com", "wsin qlez cfmy stcu"),
            //    EnableSsl = true,
            //    Timeout = 10000,

            //};
            //smtpClient.Send(mailMessage);


            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("otsordertesting@gmail.com");
            email.To.Add(MailboxAddress.Parse(emailid));
            email.Subject = subject;
            var builder = new BodyBuilder();
            //if (mailRequest.Attachments != null)
            //{
            //    byte[] fileBytes;
            //    foreach (var file in mailRequest.Attachments)
            //    {
            //        if (file.Length > 0)
            //        {
            //            using (var ms = new MemoryStream())
            //            {
            //                file.CopyTo(ms);
            //                fileBytes = ms.ToArray();
            //            }
            //            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
            //        }
            //    }
            //}
            builder.HtmlBody = msg;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("otsordertesting@gmail.com", "wsin qlez cfmy stcu");
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
        


