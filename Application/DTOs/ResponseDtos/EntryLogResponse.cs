using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.ResponseDtos
{
    public class EntryLogResponse
    {
        [Key]
        public int Id { get; set; }

        public int? MemberId { get; set; }

        public Member? Member { get; set; }

        public int LabId { get; set; }

        [Required]
        public Lab Lab { get; set; }

        [Required]
        public DateTime EventTime { get; set; }

        [Required]
        public EntryStatus status { get; set; }

        public byte[]? FaceImage { get; set; }
    }
}
