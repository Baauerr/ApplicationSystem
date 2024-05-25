using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Document
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

    public class EditEducationDocumentFormRPC
    {
        public EducationDocumentFormDTO EducationDocumentInfo { get; set; }
        public Guid UserId { get; set; }
    }

}
