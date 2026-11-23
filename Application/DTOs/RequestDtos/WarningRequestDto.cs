using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Domain.Entities;

namespace Application.DTOs.RequestDtos
{
    public class WarningRequestDto
    {


        [Required]
        public int LabId { get; set; }

        public Lab? Lab { get; set; }

        [Required]
        public Alert warning { get; set; }
    }
}
