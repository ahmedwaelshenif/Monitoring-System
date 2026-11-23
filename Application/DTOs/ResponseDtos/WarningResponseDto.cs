using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.ResponseDtos
{
    public class WarningResponseDto
    {
        public int Id { get; set; }

        public int? LabId { get; set; }

        public Lab? Lab { get; set; }

        [Required]
        public Alert warning { get; set; }
    }
}
