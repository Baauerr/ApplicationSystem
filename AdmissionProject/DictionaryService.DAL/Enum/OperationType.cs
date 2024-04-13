using System.Text.Json.Serialization;

namespace DictionaryService.DAL.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OperationType
    {
        DocumentType,
        EducationLevel,
        Faculty,
        Program
    }
}
