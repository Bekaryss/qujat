using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Qujat.Core.Data;
using Qujat.Core.Data.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qujat.Api.Services
{
    public interface IEmailSenderProvider
    {
        Task<bool> SendEmail(string email, string subject, string text = null,
            string attachmentFileName = null, byte[] attachment = null, CancellationToken ct = default);
    }

    public class GmailSmtpEmailSenderProvider : IEmailSenderProvider
    {
        public async Task<bool> SendEmail(string email, string subject, string text = null, 
            string attachmentFileName = null, byte[] attachment = null, CancellationToken ct = default)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("qujat.kz", "qujat.dev@gmail.com"));
                message.To.Add(new MailboxAddress(email, email));
                message.Subject = subject;

                var builder = new BodyBuilder();
                builder.TextBody = @"";
                builder.Attachments.Add(attachmentFileName, attachment);
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, false, ct);
                await client.AuthenticateAsync("qujat.dev@gmail.com", "iccvvadulkcvkims", ct);
                await client.SendAsync(message);
                await client.DisconnectAsync(true, ct);
            }
            catch(Exception ex)
            {
                throw;
            }
            
            return true;
        }
    }

    public class EmailSenderProviderFactory(
        IServiceProvider serviceProvider,
        ApplicationDbContext database)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ApplicationDbContext _database = database;

        public async Task<IEmailSenderProvider> GetEmailSenderProvider()
        {
            var configuration = await _database.ApplicationConfiguration.FirstOrDefaultAsync();

            var emailSenderProviderType = configuration.EmailSenderProviderType;

            if (emailSenderProviderType == EmailSenderProvider.GmailSmtp)
            {
                return new GmailSmtpEmailSenderProvider();
            }

            throw new NotImplementedException();
        }
    }
}
