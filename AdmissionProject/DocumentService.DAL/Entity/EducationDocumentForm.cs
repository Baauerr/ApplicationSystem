﻿using System;

namespace DocumentService.DAL.Entity
{
    public class EducationDocumentForm
    {
        public Guid OwnerId { get; set; }
        public string EducationLevelId { get; set; }
        public string Name { get; set; }
        public Guid? fileId { get; set; } = Guid.Empty;
    }
}