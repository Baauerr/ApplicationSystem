using EntranceService.Common.DTO;

namespace EntranceService.Common.Interface
{
    public interface IDocumentService
    {
        public Task<PassportInfoDTO> GetPassportInfo(Guid userId);
        public Task EditPassportInfo(PassportInfoDTO passportDTO, Guid userId);
        public Task<List<EducationDocumentDTO>> GetEducationDocumentsInfo(Guid userId);
        public Task EditEducationDocumentsInfo(EducationDocumentDTO educationDocumentDTO, Guid userId);
        public Task AddEducationDocumentsInfo(
            EducationDocumentDTO educationDocumentDTO, Guid userId);
        public Task AddPassportInfo(
            PassportInfoDTO passportDTO, Guid userId);
    }
}
