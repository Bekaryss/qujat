using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Qujat.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qujat.Core.Services
{
    public interface ICurrentUserProvider
    {
        CurrentRqUser GetCurrentUser();
    }

    public class HttpContextCurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccessTokenDecoder _accessTokenDecoder;
        private const string AuthorizationHeaderName = "Authorization";
        private CurrentRqUser _cachedCurrentRqUser;

        public HttpContextCurrentUserProvider(
            IHttpContextAccessor httpContextAccessor,
            IAccessTokenDecoder accessTokenDecoder)
        {
            _httpContextAccessor = httpContextAccessor;
            _accessTokenDecoder = accessTokenDecoder;
        }

        public CurrentRqUser GetCurrentUser()
        {
            if (_cachedCurrentRqUser != null)
                return _cachedCurrentRqUser;

            if (_httpContextAccessor.HttpContext.Request.Headers
                .TryGetValue(AuthorizationHeaderName, out StringValues authorizationTokenStr))
            {
                var decodedAccessTokenData = _accessTokenDecoder.DecodeAccessToken(authorizationTokenStr.FirstOrDefault());
                if (decodedAccessTokenData == null)
                    return null;

                var response = new CurrentRqUser
                {
                    UserId = decodedAccessTokenData.UserId
                };

                _cachedCurrentRqUser = response;
                return _cachedCurrentRqUser;
            }

            return null;
        }
    }
}
