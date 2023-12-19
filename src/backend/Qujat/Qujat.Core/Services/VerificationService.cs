using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Qujat.Core.Exceptions;

namespace Qujat.Core.Services
{
    public enum OneTimeVerificationTokenType
    {
        SignUpConfirmation,
        PasswordResetConfirmation
    }

    public record VerificationLinkedData(
        long ApplicationUserId,
        string ConfirmationCode,
        OneTimeVerificationTokenType VerificationTokenType,
        string LinkedOneTimeToken);

    public class VerificationService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<VerificationService> _logger;

        /// <summary>
        /// verification-id-{verificationId}
        /// </summary>
        private const string VERIFICATION_KEY_PLACEHOLDER = "verification-{0}";
        private readonly TimeSpan AbsoluteExpirationForVerificationOperations = TimeSpan.FromMinutes(15);

        public VerificationService(
            IMemoryCache memoryCache, ILogger<VerificationService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<string> CreateOrUpdateVerificationData(
            long applicationUserId, string confirmationCode)
        {
            var verificationId = Guid.NewGuid().ToString();
            var key = string.Format(VERIFICATION_KEY_PLACEHOLDER, verificationId);
            var value = new VerificationLinkedData(applicationUserId, confirmationCode, OneTimeVerificationTokenType.SignUpConfirmation, null);

            var expiration = DateTimeOffset.Now.Add(AbsoluteExpirationForVerificationOperations);

            _memoryCache.Set(key, value, expiration);
            return Task.FromResult(verificationId);
        }

        public Task<string> UpdateVerificationDataWithOneTimeToken(string verificationId, string oneTimeToken)
        {
            var key = string.Format(VERIFICATION_KEY_PLACEHOLDER, verificationId);
            var value = _memoryCache.Get<VerificationLinkedData>(key) ?? throw new ResourceNotFoundException();

            var updatedValue = value with { LinkedOneTimeToken = oneTimeToken };

            DeleteVerificationData(verificationId);

            var expiration = DateTimeOffset.Now.Add(AbsoluteExpirationForVerificationOperations);
            _memoryCache.Set(key, updatedValue, expiration);

            return Task.FromResult(verificationId);
        }

        public Task<string> UpdateVerificationDataWithConfirmationCode(
            string verificationId, string confirmationCode)
        {
            var key = string.Format(VERIFICATION_KEY_PLACEHOLDER, verificationId);
            var value = _memoryCache.Get<VerificationLinkedData>(key) ?? throw new ResourceNotFoundException();

            var updatedValue = value with { ConfirmationCode = confirmationCode };

            DeleteVerificationData(verificationId);

            var expiration = DateTimeOffset.Now.Add(AbsoluteExpirationForVerificationOperations);
            _memoryCache.Set(key, updatedValue, expiration);

            return Task.FromResult(verificationId);
        }

        public Task<VerificationLinkedData> GetVerificationData(string verificationId)
        {
            var key = string.Format(VERIFICATION_KEY_PLACEHOLDER, verificationId);
            var value = _memoryCache.Get<VerificationLinkedData>(key);

            return Task.FromResult(value);
        }

        public Task DeleteVerificationData(string verificationId)
        {
            var key = string.Format(VERIFICATION_KEY_PLACEHOLDER, verificationId);
            _memoryCache.Remove(key);

            return Task.CompletedTask;
        }
    }
}
