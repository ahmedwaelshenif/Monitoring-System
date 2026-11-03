using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class LabRequest
    {
        [Required]
        public string Name {  get; set; }

        [Required]
        public int Capacity {  get; set; }
    }
}
