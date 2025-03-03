﻿using Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace DictionaryService.DAL.Entities
{
    public class ImportHistory
    {
        [Key]
        public Guid Id {  get; set; }
        public required ImportTypes OperationType { get; set; }
        public required ImportStatus ImportStatus { get; set; }
        public required DateTime OperationTime { get; set; }
        public required Guid UserId { get; set; }
    }
}
