using System.Text.Json.Serialization;

namespace DictionaryService.Common.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Language
    {
        English, 
        Русский
    }
}
