using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class DashboardAccountRequestDto
    {

        [Required]
        public string FullName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public Role Role { get; set; }

        public List<int> LabIds { get; set; } = new List<int>();


    }
}
