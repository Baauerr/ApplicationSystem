using EntranceService.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.Common.Interface
{
    public interface IEntranceService
    {
        public Task DeleteProgram(Guid programId, Guid applicationId);
        public Task ChangeProgramPriority(int programPriority, Guid programId, Guid applicationId);
        public Task AddProgramToApplication(Guid programId, Guid applicationId);
        public Task ChangeApplicationStatus(string status, Guid applicationId);
        public Task GetApplications();
        public Task CreateApplication(Guid userId, string token, CreateApplicationDTO applicationDTO);
        
    }
}
