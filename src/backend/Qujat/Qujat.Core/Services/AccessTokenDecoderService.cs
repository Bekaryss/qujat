using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Qujat.Core.DTOs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace Qujat.Core.Services
{
    public interface IAccessTokenDecoder
    {
        CurrentRqUser DecodeAccessToken(string accessToken);
    }

    public class DefaultAccessTokenDecoder : IAccessTokenDecoder
    {
        private readonly ILogger<DefaultAccessTokenDecoder> _logger;
        public DefaultAccessTokenDecoder(ILogger<DefaultAccessTokenDecoder> logger)
        {
            _logger = logger;
        }

        public CurrentRqUser DecodeAccessToken(string accessToken)
        {
            if (accessToken == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("change-later-this-place-ket-must-be-greatger-than-256-symbols-here-remember-it");

            var response = default(CurrentRqUser);

            try
            {
                tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                response = new CurrentRqUser
                {
                    UserId = long.Parse(jwtToken.Claims.First(x => x.Type == "userId").Value),
                    //VerificationTokenType = verificationTokenTypeParsed ? verificatioTokenType : null
                };

            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Exception with message {message} while decoding access token [{accessToken}]", ex.Message, accessToken);
                return null;
            }

            return response;
        }
    }
}
