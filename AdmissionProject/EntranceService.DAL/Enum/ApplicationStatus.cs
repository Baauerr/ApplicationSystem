using System.Text.Json.Serialization;

namespace EntranceService.DAL.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApplicationStatus
    {
        Pending, 
        InProcess,
        Closed
    }
}
