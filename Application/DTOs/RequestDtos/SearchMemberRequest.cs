using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class SearchMemberRequest
    {
        public int? Id { get; set; }
        public string? FullName { get; set; }

        public string? Email { get; set; }

        [Range(1, 4)]
        public int? AcademicYear { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
