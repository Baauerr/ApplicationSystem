﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.DAL.Entity
{
    public class ApplicationPrograms
    {
        public Guid ProgramId { get; set; }
        public Guid ApplicationId { get; set; }
        public string ProgramName { get; set; }
        public string FacultyId { get; set; }
        public string FacultyName { get; set; }
        public int Priority { get; set; }
    }
}
