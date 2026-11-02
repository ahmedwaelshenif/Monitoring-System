using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.ResponseDtos
{
    public class DashboardAccountResponse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public Role Role { get; set; }


        public bool IsActive { get; set; } = true;
    }
}
