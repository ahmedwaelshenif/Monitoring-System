using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public class EntryLog
    {
        [Key]
        public int Id { get; set; }

        public int? VisitorId { get; set; }

      
        public Visitor? Visitor { get; set; }

        public int ?LabId { get; set; }

        [ForeignKey(nameof(LabId))]
        public Lab ?Lab { get; set; }

        [Required]
        public DateTime EventTime { get; set; }

        [Required]
        public EntryStatus Status { get; set; }
    }
}
