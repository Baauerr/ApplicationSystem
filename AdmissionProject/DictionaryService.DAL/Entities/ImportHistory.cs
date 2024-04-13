
using DictionaryService.DAL.Enum;

namespace DictionaryService.DAL.Entities
{
    public class ImportHistory
    {
        public required OperationType OperationType { get; set; }
        public required ImportStatus ImportStatus { get; set; }
        public required string userId { get; set; }

    }
}
