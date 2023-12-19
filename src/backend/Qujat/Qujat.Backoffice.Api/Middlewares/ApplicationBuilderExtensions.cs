using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Qujat.Core.DTOs.Shared;
using Qujat.Core.Exceptions;
using System.Linq;
using System.Net;

namespace Qujat.Backoffice.Api.Middlewares
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder MapProblemDetails(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>()!;
                    var exception = exceptionHandlerPathFeature.Error;

                    switch (exception)
                    {
                        case ValidationErrorsException ex:
                            var responseModel = new ApiResponse
                            {
                                ResponseType = ApiResponseRuntimeType.Error,
                                ResponseClientMessage = ex.ClientMessage,
                                ResponseInternalMessage = ex.InternalMessage,
                                RequestValidationErrors = ex.Errors.ToArray(),
                                RequestSucceeded = false
                            };
                            
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            context.Response.ContentType = "application/problem+json";
                            
                            await context.Response.WriteAsync(
                                JsonConvert.SerializeObject(responseModel, Formatting.Indented));
                            break;

                        case ValidationException ex:
                            var errors = ex.Errors.Select(
                                x => new RequestValidationErrorDto(x.PropertyName, x.ErrorMessage, x.ErrorCode));
                            var domainEx = new ValidationErrorsException(errors);

                            responseModel = new ApiResponse
                            {
                                ResponseType = ApiResponseRuntimeType.Error,
                                ResponseClientMessage = domainEx.ClientMessage,
                                ResponseInternalMessage = domainEx.InternalMessage,
                                RequestValidationErrors = domainEx.Errors.ToArray(),
                                RequestSucceeded = false
                            };

                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            context.Response.ContentType = "application/problem+json";

                            await context.Response.WriteAsync(
                                JsonConvert.SerializeObject(responseModel, Formatting.Indented));
                            break;

                        case DomainException ex:
                            responseModel = new ApiResponse
                            {
                                ResponseType = ApiResponseRuntimeType.Error,
                                ResponseClientMessage = ex.ClientMessage,
                                ResponseInternalMessage = ex.InternalMessage,
                                RequestSucceeded = false
                            };

                            context.Response.StatusCode = (int)ex.CorrespondedHttpStatusCode;
                            context.Response.ContentType = "application/problem+json";

                            await context.Response.WriteAsync(
                                JsonConvert.SerializeObject(responseModel, Formatting.Indented));
                            break;

                        default:
                            responseModel = new ApiResponse
                            {
                                ResponseType = ApiResponseRuntimeType.Error,
                                ResponseClientMessage = "Что-то пошло не так, команда разработки уже разбирается",
                                ResponseInternalMessage = exception.Message,
                                RequestSucceeded = false,
                            };

                            context.Response.StatusCode = 500;
                            context.Response.ContentType = "application/problem+json";

                            await context.Response.WriteAsync(
                                JsonConvert.SerializeObject(responseModel, Formatting.Indented));
                            break;
                    }
                });
            });

            return app;
        }
    }
}
