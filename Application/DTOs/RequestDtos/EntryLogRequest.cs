using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class EntryLogRequest
    {
        public int? MemberId { get; set; }

        public Member? Member { get; set; }

        public int LabId { get; set; }

        public Lab? Lab { get; set; }

        [Required]
        public DateTime EventTime { get; set; }

        [Required]
        public EntryStatus status { get; set; }
    }
}
