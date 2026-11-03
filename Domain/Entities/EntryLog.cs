using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using System.Reflection.Emit;

namespace Domain.Entities
{
    public class EntryLog
    {
        [Key]
        public int Id { get; set; }

        public Member? Member { get; set; }

        [Required]
        public Lab Lab { get; set; }

        [Required]
        public DateTime EventTime { get; set; }

        [Required]
        public EntryStatus status { get; set; }


    }
}
