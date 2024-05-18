using System.Text.Json.Serialization;

namespace Common.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApplicationStatus
    {
        Created, 
        InProcess,
        Approved, 
        Rejected,
        Closed
    }
}
