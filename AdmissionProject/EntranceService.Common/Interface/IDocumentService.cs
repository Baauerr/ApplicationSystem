using EntranceService.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.Common.Interface
{
    public interface IDocumentService
    {
        public Task GetPassportInfo(Guid userId, Guid applicationId);
        public Task EditPassportInfo(Guid userId, Guid applicationId);
        public Task GetEducationDocumentInfo(Guid userId, Guid applicationId);
        public Task EditEducationDocumentInfo(Guid userId, Guid applicationId);
        public Task AddEducationDocumentInfo(
            EducationDocumentDTO educationDocumentDTO, Guid userId);
        public Task AddPassportInfo(
            PassportInfoDTO passportDTO, Guid userId);
    }
}
