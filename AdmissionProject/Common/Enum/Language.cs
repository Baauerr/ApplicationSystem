using System.Text.Json.Serialization;

namespace Common.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Language
    {
        English, 
        Русский
    }
}
