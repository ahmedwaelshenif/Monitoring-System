using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
namespace Domain.Entities
{
    public class Warning
    {
        [Key]
        public int Id { get; set; }

        public int ?LabId { get; set; }

        public Lab ?Lab { get; set; }

        [Required]
        public Alert warning { get; set; } 
    }
}
