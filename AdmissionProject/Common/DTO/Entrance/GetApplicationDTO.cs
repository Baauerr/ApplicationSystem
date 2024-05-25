using Common.DTO.Dictionary;
using Common.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Entrance
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
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
    }
    public class ApplicationsResponseDTO
    {
        public List<GetApplicationDTO> Applications { get; set; }
        public Pagination Pagination { get; set; }
    }

    public class ApplicationFiltersDTO
    {
        public string? entrantName { get; set; }

        public Guid? programsGuid { get; set; }

        public List<string>? faculties { get; set; }

        public ApplicationStatus? status { get; set; }

        public bool? hasManager { get; set; }
        public bool? onlyMyManaging {  get; set; }

        public Guid? managerId { get; set; }

        public SortingTypes? sortingTypes { get; set; }

        public int page { get; set; } = 1;

        public int pageSize { get; set; } = 10;
        public Guid myId { get; set; }
    }
}
