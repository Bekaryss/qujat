using Microsoft.IdentityModel.Tokens;
using Qujat.Core.DTOs.Shared;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Qujat.Core.Services
{
    public interface IAccessTokenGenerator
    {
        Task<AccessToken> GenerateAccessToken(long applicationUserId, bool phoneNumberConfirmed = false);
        Task<AccessToken> GenerateOneTimeVerificationToken(
            long applicationUserId, OneTimeVerificationTokenType verificationTokenType);
    }

    public class DefaultJwtAccessTokenGenerator : IAccessTokenGenerator
    {
        public Task<AccessToken> GenerateAccessToken(
            long applicationUserId, bool phoneNumberConfirmed = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("change-later-this-place-ket-must-be-greatger-than-256-symbols-here-remember-it");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userId", applicationUserId.ToString()) }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(new AccessToken(tokenHandler.WriteToken(token)));
        }


        public Task<AccessToken> GenerateOneTimeVerificationToken(
            long applicationUserId, OneTimeVerificationTokenType verificationTokenType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("change-later-this-place");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userId", applicationUserId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(new AccessToken(tokenHandler.WriteToken(token)));
        }
    }
}
