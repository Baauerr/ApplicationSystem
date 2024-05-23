
using Common.DTO.Dictionary;
using Common.Enum;

namespace DictionaryService.Common.Interfaces
{
    public interface IImportService
    {
        public Task ImportDictionary(ImportTypes operationType, Guid userId);
        public Task<AllImportHistoryDTO> GetImportHistory();
    }

}
