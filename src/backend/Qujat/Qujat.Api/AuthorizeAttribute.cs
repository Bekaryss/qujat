using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Qujat.Core.Services;
using System;
using System.Linq;
using Qujat.Core.DTOs.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Qujat.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeByBackofficeApiKeyAttribute : Attribute, IAuthorizationFilter
    {
        private const string AuthorizationHeaderName = "Authorization";
        public const string BackofficeApiKey = "5a9a759d465f4b06896e67dfa6055554";
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            var httpContextAccessor = context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();

            if (httpContextAccessor.HttpContext.Request.Headers
               .TryGetValue(AuthorizationHeaderName, out StringValues authorizationTokenStr))
            {
                if (!authorizationTokenStr[0].Equals(BackofficeApiKey, StringComparison.Ordinal))
                {
                    context.Result = new JsonResult(
                        new ApiResponse
                        {
                            ResponseClientMessage = "Unauthorized",
                            ResponseInternalMessage = "Unauthorized",
                            RequestSucceeded = false
                        })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeByAccessTokenAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            var currentUserProvider = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserProvider>();
            var currentUser = currentUserProvider.GetCurrentUser();

            if (currentUser == null)
            {
                context.Result = new JsonResult(
                    new ApiResponse
                    {
                        ResponseClientMessage = "Unauthorized",
                        ResponseInternalMessage = "Unauthorized",
                        RequestSucceeded = false
                    })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeByPasswordResetOtt : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            var currentUserProvider = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserProvider>();
            var currentUser = currentUserProvider.GetCurrentUser();

            if (currentUser == null)
            {
                context.Result = new JsonResult(
                    new ApiResponse
                    {
                        ResponseClientMessage = "Unauthorized",
                        ResponseInternalMessage = "Unauthorized",
                        RequestSucceeded = false
                    })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}
