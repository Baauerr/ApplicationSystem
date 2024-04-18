using DictionaryService.Common.DTO;
using DictionaryService.Common.Enums;
using DictionaryService.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.Common.Interfaces
{
    public interface IDictionaryInfoService
    {
        public Task<ProgramResponseDTO> GetPrograms(
            string? name,
            string? code,
            List<Language>? language,
            List<string>? educationForm,
            List<string>? educationLevel,
            List<string>? faculty,
            int page = 1,
            int pageSize = 10
            );
        public Task<FacultiesResponseDTO> GetFaculties(string? facultyName);
        public Task<DocumentTypeResponseDTO> GetDocumentTypes(string? documentTypeName);
        public Task<EducationLevelResponseDTO> GetEducationLevel(string? educationLevelName);
        public Task<List<ImportHistory>> GetImportHistory();
    }
}
