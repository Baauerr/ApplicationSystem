using Common.DTO.Dictionary;

namespace DocumentService.Common.Interface
{
    public interface IRequestService
    {
        public Task<EducationLevelResponseDTO> GetEducationLevels();
    }
}
