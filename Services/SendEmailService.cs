using FormsNet.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FormsNet.Services
{
	public class SendEmailService : ISendEmailService
	{
		public SendEmailService()
		{

		}

		public async Task<int> SendEmail(string[] tos, string subject, string body)
		{
            int result = 0;

            MailMessage message = new MailMessage();
            message.From = new MailAddress("no-reply@test.com");
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            if (tos != null)
            {
                for (int i = 0; i < tos.Length; i++)
                {
                    string to = tos[i];
                    if (to != null && !string.IsNullOrWhiteSpace(to.Trim()))
                    {
                        message.Bcc.Add(new MailAddress(to));
                    }
                }
            }



            SmtpClient client = new SmtpClient();
            client.Host = "";
            // Credentials are necessary if the server requires the client
            // to authenticate before it will send email on the client's behalf.
            client.UseDefaultCredentials = true;

            try
            {
                 await client.SendMailAsync(message);
                result = 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    ex.ToString());
            }

            return result;
        }


    }
}
