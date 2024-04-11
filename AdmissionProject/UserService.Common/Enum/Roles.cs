using System.Text.Json.Serialization;

namespace UserService.Common.Enum
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
