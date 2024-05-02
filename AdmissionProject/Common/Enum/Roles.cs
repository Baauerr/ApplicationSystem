using System.Text.Json.Serialization;

namespace Common.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Roles
    {
        ADMINISTRATOR,
        MANAGER,
        MAINMANAGER,
        USER,
        ENTRANT
    }
}
