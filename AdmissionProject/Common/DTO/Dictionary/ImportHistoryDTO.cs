using Common.Enum;
using Microsoft.OpenApi.Models;

namespace Common.DTO.Dictionary
{
    public class HistoryDTO
    {
        public required ImportTypes OperationType { get; set; }
        public required ImportStatus ImportStatus { get; set; }
        public required DateTime OperationTime { get; set; }
    }

    public class AllImportHistoryDTO
    {
        public ImportTypes Types { get; set; }
        public List<HistoryDTO> History { get; set;} = new List<HistoryDTO>();
    }
}
