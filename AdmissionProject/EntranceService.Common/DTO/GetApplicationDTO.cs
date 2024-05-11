using Common.DTO.Dictionary;
using EntranceService.DAL.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.Common.DTO
{
    public class GetApplicationDTO
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string Citizenship { get; set; }
        public DateTime LastChangeDate { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public Guid ManagerId { get; set; } 
        public List<GetProgramDTO> Programs { get; set; }
    }
    public class ApplicationsResponseDTO
    {
        public List<GetApplicationDTO> Applications { get; set; }
        public Pagination Pagination { get; set; }
    }
}
