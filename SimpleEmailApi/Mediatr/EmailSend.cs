using MediatR;

using Microsoft.Extensions.Hosting;

using System.Net.Mail;
using System.Net;
using System.Threading.Channels;
using MimeKit;
using MailKit.Security;
using SimpleEmailApi.Configuration;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace SimpleEmailApi.Mediatr
{
    public static class EmailSend
    {

        public record Request([Required] string Subject, [Required] string Body, [Required] string ToEmail, string ReplyToEmail) : IRequest<Response>;
        public record Response(string Status);
        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly EmailOptions emailOptions;
            public Handler(IOptions<EmailOptions> emailOptions)
            {
                this.emailOptions = emailOptions.Value;
            }



            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                //validate request
                //Validate(request);

                //Log request if needed
                //Save(request);


                var message = new MimeMessage();
                var builder = new BodyBuilder();

                message.From.Add(new MailboxAddress(emailOptions.DisplayName, emailOptions.UserName));
                message.To.Add(new MailboxAddress(request.ToEmail, request.ToEmail));

                message.Subject = request.Subject;
                if (!string.IsNullOrEmpty(request.ReplyToEmail))
                    message.ReplyTo.Add(new MailboxAddress(request.ReplyToEmail, request.ReplyToEmail));

                builder.HtmlBody = request.Body;

                //if (request.Attachments.Any())
                //{
                //    foreach (var item in request.Attachments)
                //    {
                //        using (var client = new HttpClient())
                //        {
                //            var stream = await client.GetStreamAsync(item.Url);
                //            //stream is disposed allong with mail message
                //            var mem = new MemoryStream();
                //            stream.CopyTo(mem);
                //            builder.Attachments.Add(item.Name, mem);
                //        }
                //    }
                //}
                message.Body = builder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    try
                    {
                        client.ServerCertificateValidationCallback = (sender, cert, chain, err) => true;

                        await client.ConnectAsync(emailOptions.Server, emailOptions.Port, true);

                        await client.AuthenticateAsync(emailOptions.UserName, emailOptions.Password);
                        await client.SendAsync(message);
                        return new Response("Yay");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        await client.DisconnectAsync(true);
                    }

                }

            }
        }
    }

}
