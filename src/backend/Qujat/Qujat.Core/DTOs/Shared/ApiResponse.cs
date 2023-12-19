using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Qujat.Core.DTOs.Shared
{
    public record RequestValidationErrorDto(
        [property: JsonProperty("propertyName")] string PropertyName,
        [property: JsonProperty("propertyErrorMessage")] string PropertyErrorMessage,
        [property: JsonProperty("propertyErrorCode")] string PropertyErrorCode);

    public class ApiResponse
    {
        [JsonProperty("responseType")]
        public ApiResponseRuntimeType ResponseType { get; set; }

        [JsonProperty("requestSucceeded")]
        public bool RequestSucceeded { get; set; }

        [JsonProperty("responseClientMessage")]
        public string ResponseClientMessage { get; set; }

        [JsonProperty("responseInternalMessage")]
        public string ResponseInternalMessage { get; set; }

        [JsonProperty("requestValidationErrors")]
        public RequestValidationErrorDto[] RequestValidationErrors { get; set; }

        [JsonProperty("isPaginatedResponse")]
        public bool IsPaginatedResponse { get; set; } = false;

        [JsonProperty("pageIndex")]
        public long? PageIndex { get; set; } = null;

        [JsonProperty("pageSize")]
        public long? PageSize { get; set; } = null;

        [JsonProperty("totalSize")]
        public long? TotalSize { get; set; } = null;

        public static ApiResponse Ok() => new()
        {
            RequestSucceeded = true,
            ResponseType = ApiResponseRuntimeType.Idle
        };

        public static ApiResponse Fail() => new()
        {
            RequestSucceeded = false,
            ResponseType = ApiResponseRuntimeType.Error
        };
    }


    public class ApiResponse<TResponseData>(TResponseData data) : ApiResponse
    {
        [JsonProperty("responseData")]
        public TResponseData ResponseData { get; set; } = data;

        public static ApiResponse<TResponseData> Ok(TResponseData data) => new(data)
        {
            RequestSucceeded = true,
            ResponseType = ApiResponseRuntimeType.Data,
            IsPaginatedResponse = false
        };

        public static ApiResponse<TResponseData> Ok(TResponseData data, int pageIndex, int pageSize, int totalSize = 0) => new(data)
        {
            RequestSucceeded = true,
            ResponseType = ApiResponseRuntimeType.Data,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalSize = totalSize,
            IsPaginatedResponse = true
        };

        public static ApiResponse<TResponseData> Fail(TResponseData data) => new(data)
        {
            RequestSucceeded = false,
            ResponseType = ApiResponseRuntimeType.Error,
            IsPaginatedResponse = false
        };
    }
}
