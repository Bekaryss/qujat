using Newtonsoft.Json;
using Qujat.Core.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Net;

namespace Qujat.Core.Exceptions
{
    public class DomainException : Exception
    {
        [JsonProperty("correspondedHttpStatusCode")]
        public HttpStatusCode CorrespondedHttpStatusCode = HttpStatusCode.BadRequest;

        [JsonProperty("clientMessage")]
        public string ClientMessage { get; set; }

        [JsonProperty("internalMessage")]
        public string InternalMessage { get; set; }

        [JsonProperty("correlationId")]
        public string CorrelationId { get; set; }

        public DomainException(string clientMessage = null, string internalMessage = null)
        {
            ClientMessage = clientMessage;
            InternalMessage = internalMessage;
        }
    }


    public class ResourceNotFoundException : DomainException
    {
        public ResourceNotFoundException(
            string clientMessage = "Запрашиваемый ресурс не найден, возможно он был удален", 
            string internalMessage = "Requested data not found, it may has been deleted") :
            base(clientMessage, internalMessage)
        {
            CorrespondedHttpStatusCode = HttpStatusCode.NotFound;
        }
    }

    public class BadRequestException : DomainException
    {
        public BadRequestException(
            string clientMessage = "Ошибка обработки запроса, попробуйте еще раз или обратитесь в службу поддержки", 
            string internalMessage = "Request completed with client error, see inner details and try to fix the request") :
            base(clientMessage, internalMessage)
        {
        }
    }


    public class EmptyRqBodyException : BadRequestException
    {
        public EmptyRqBodyException() : base(
            "Тело запроса не может быть пустым", 
            "Request body cannot be null in the request, see documentation")
        {
        }
    }

    public class ValidationErrorsException : DomainException
    {
        public ValidationErrorsException(IEnumerable<RequestValidationErrorDto> errors) :
            base("One or more validation errors have occurred")
        {
            Errors = errors;
        }

        public ValidationErrorsException(params RequestValidationErrorDto[] errors) :
            this((IEnumerable<RequestValidationErrorDto>)errors)
        {
        }

        public ValidationErrorsException(string field, string message, string code) :
            this(new RequestValidationErrorDto(field, message, code))
        {
        }

        public IEnumerable<RequestValidationErrorDto> Errors { get; }
    }
}
