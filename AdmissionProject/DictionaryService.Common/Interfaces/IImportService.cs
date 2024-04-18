
using DictionaryService.DAL.Enum;

namespace DictionaryService.Common.Interfaces
{
    public interface IImportService
    {
        public Task ImportDictionary(OperationType operationType, Guid userId);
    }
}
