using Common.DTO.Document;

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

        public Task AddPassportInfo(
            PassportFormDTO passportDTO, Guid userId);
    }
}
