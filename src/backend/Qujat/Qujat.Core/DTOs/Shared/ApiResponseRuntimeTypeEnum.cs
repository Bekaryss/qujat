using System.Runtime.Serialization;

namespace Qujat.Core.DTOs.Shared
{
    public enum ApiResponseRuntimeType
    {
        [EnumMember(Value = "data")]
        Data = 1,
        [EnumMember(Value = "idle")]
        Idle,
        [EnumMember(Value = "error")]
        Error
    }
}
