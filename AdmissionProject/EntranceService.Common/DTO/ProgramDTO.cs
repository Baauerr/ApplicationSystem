﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.Common.DTO
{
    public class ProgramDTO
    {
        public Guid ProgramId { get; set; }
        public string FacultyId { get; set; }
        public int ProgramPriority { get; set; }
    }
    public class AddProgramDTO
    {
        public required List<ProgramDTO> programs { get; set; }
        public Guid ApplicationId { get; set; }
    }

    public class ProgramChangePriorityDTO
    {
        public Guid ApplicationId { get; set; }
        public Guid ProgramGuid { get; set; }
        public int NewPriority { get; set; }
        public int OldPriority { get; set; }
    }

    public class DeleteProgramDTO
    {
        public Guid ProgramId { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
