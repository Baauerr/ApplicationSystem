using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.DAL.Entity
{
    public class EducationDocumentData
    {

        public Guid ownerId {  get; set; }
        public Guid EducationDocumentId { get; set; }
        public string name {  get; set; }
    }
}
