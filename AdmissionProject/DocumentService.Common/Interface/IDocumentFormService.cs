using DocumentService.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Common.Interface
{
    public interface IDocumentFormService
    {
        public Task<GetPassportFormDTO> GetPassportInfo(Guid userId);
        public Task EditPassportInfo(PassportFormDTO passportDTO, Guid userId);
        public Task<List<GetEducationDocumentFormDTO>> GetEducationDocumentsInfo(Guid userId);
        public Task EditEducationDocumentsInfo(EducationDocumentFormDTO educationDocumentDTO, Guid userId);
        public Task AddEducationDocumentsInfo(
            EducationDocumentFormDTO educationDocumentDTO, Guid userId);
        public Task DeleteEducationDocumentsInfo(
            DeleteEducationFormDTO educationDocumentDTO, Guid userId);

        public Task AddPassportInfo(
            PassportFormDTO passportDTO, Guid userId);
        public Task DeletePassportInfo(Guid userId);
    }
}
