using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.DAL.Entity
{
    public class EducationDocumentData
    {
        public Guid OwnerId {  get; set; }
        public Guid EducationDocumentId { get; set; }
        public string EducationLevelId { get; set; }
        public string Name {  get; set; }
    }
}
