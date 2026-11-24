using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class VisitorFilterDto : PagedRequestDto
    {
        public int? Id { get; set; }

        public int? MemberId { get; set; }
        public string? FullName { get; set; }

        public string? Email { get; set; }

        [Range(1, 4)]
        public int? AcademicYear { get; set; }

        public bool? Active { get; set; } = true;



    }
}
