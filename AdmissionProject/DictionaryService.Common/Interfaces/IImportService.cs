
namespace DictionaryService.Common.Interfaces
{
    public interface IImportService
    {
        public Task ImportEducationLevels();
        public Task ImportDocumentTypes();
        public Task ImportFaculties();
        public Task ImportPrograms();
        public Task GetImportStatus();
    }
}
