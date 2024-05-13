using DocumentService.Common.DTO;

namespace DocumentService.Common.Interface
{
    public interface IDocumentFormService
    {
        public Task<GetPassportFormDTO> GetPassportInfo(Guid userId);
        public Task EditPassportInfo(PassportFormDTO passportDTO, Guid userId);
        public Task<GetEducationDocumentFormDTO> GetEducationDocumentInfo(Guid userId);
        public Task EditEducationDocumentInfo(EducationDocumentFormDTO educationDocumentDTO, Guid userId);
        public Task AddEducationDocumentInfo(
            EducationDocumentFormDTO educationDocumentDTO, Guid userId);
        public Task DeleteEducationDocumentInfo(
            DeleteEducationFormDTO educationDocumentDTO, Guid userId);

        public Task AddPassportInfo(
            PassportFormDTO passportDTO, Guid userId);
        public Task DeletePassportInfo(Guid userId);
    }
}
