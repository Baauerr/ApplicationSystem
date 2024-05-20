using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Common.DTO
{
    public class EducationDocumentFormDTO
    {
        public string Name { get; set; }
        public string EducationLevelId { get; set; }
    }

    public class GetEducationDocumentFormDTO: EducationDocumentFormDTO
    {
        public string EducationLevelName { get; set; }
    }
}
