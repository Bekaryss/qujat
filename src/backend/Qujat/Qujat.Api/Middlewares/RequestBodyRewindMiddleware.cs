using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Qujat.Api.Middlewares
{
    public class RequestBodyRewindMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Request.EnableBuffering();

            await next(context);
        }
    }
}
