using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class EntryLogRequestDto
    {
     
        public int? VisitorId { get; set; }

        [Required]
        public int LabId { get; set; }

        [Required]
        public DateTime EventTime { get; set; }

        [Required]
        public EntryStatus status { get; set; }
    }
}
