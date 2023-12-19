using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Qujat.Core.Services
{
    public interface ISmsNotificationService
    {
        Task<bool> SendSms(string phoneNumber, string content, CancellationToken ct);
        Task<bool> SendChatContent(string content, CancellationToken ct);
    }

    public class TelegramMockSmsNotificationProvider(ILogger<TelegramMockSmsNotificationProvider> logger) : ISmsNotificationService
    {
        private readonly ILogger<TelegramMockSmsNotificationProvider> _logger = logger;
        private const string TELEGRAM_BOT_TOKEN = "6179405293:AAEz1savs12-03zQisEyUcg6fl6GnUq2f0Y";
        private const long TELEGRAM_JUICYZONA_DEVCHANNEL = -1001642337305;
        private static readonly ITelegramBotClient _botClient = new TelegramBotClient(TELEGRAM_BOT_TOKEN);

        public async Task<bool> SendSms(string phoneNumber, string content, CancellationToken ct)
        {
            var message = $"На номер {phoneNumber} отправлено SMS {content}";
            _logger.LogInformation(message);
            //await _botClient.SendTextMessageAsync(TELEGRAM_JUICYZONA_DEVCHANNEL, message, parseMode: ParseMode.Html, cancellationToken: ct);

            return true;
        }

        public async Task<bool> SendChatContent(string content, CancellationToken ct)
        {
            _logger.LogInformation(content);
            //await _botClient.SendTextMessageAsync(TELEGRAM_JUICYZONA_DEVCHANNEL, content, parseMode: ParseMode.Html, cancellationToken: ct);
            return true;
        }

        public async Task<bool> SendException(string content, CancellationToken ct)
        {
            _logger.LogInformation(content);
            //await _botClient.SendTextMessageAsync(TELEGRAM_JUICYZONA_DEVCHANNEL, content, parseMode: ParseMode.Html, cancellationToken: ct);
            return true;
        }
    }
}
