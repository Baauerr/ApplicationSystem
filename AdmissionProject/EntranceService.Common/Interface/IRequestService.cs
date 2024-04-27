using Common.DTO.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.Common.Interface
{
    public interface IRequestService
    {
        public Task<ProgramResponseDTO> GetPrograms();
        public Task<EducationLevelResponseDTO> GetEducationLevels();
    }
}
