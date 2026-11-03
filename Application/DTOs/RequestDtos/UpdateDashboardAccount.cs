using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class UpdateDashboardAccount
    {
     
        public string? FullName { get; set; }

        [EmailAddress]
 
        public string? Email { get; set; }

        public string? Password { get; set; }

        public Role? Role { get; set; }
    }
}
