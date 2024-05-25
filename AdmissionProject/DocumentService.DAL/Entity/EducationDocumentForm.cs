using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentService.DAL.Entity
{
    public class EducationDocumentForm
    {
        [Key]
        public Guid OwnerId { get; set; }
        public string EducationLevelId { get; set; }
        public string Name { get; set; }
        public string EducationLevelName { get; set; }
        public Guid? fileId { get; set; } = Guid.Empty;
    }
}
