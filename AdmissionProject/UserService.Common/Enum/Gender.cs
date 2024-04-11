using System.Text.Json.Serialization;

namespace UserService.Common.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        FEMALE,
        MALE
    }
}
