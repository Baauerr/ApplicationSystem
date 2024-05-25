using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Document
{
    public class PassportFormDTO
    {
        public int Series { get; set; }
        public int Number { get; set; }
        public string BirthPlace { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuePlace { get; set; }
    }

    public class EditPassportFormDTORPC
    {
        public PassportFormDTO PassportInfo { get; set; }
        public Guid UserId { get; set; }

    }

    public class GetPassportFormDTO: PassportFormDTO
    {
        public Guid UserId { get; set; }
    }
    public class GetEducationDocuments
    {
        public Guid UserId { get; set; }
        public string EducationLevelId { get; set; }
    }

}
