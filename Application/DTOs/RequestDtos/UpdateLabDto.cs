using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class UpdateLabDto
    {
        public string? Name { get; set; }

        public int? Capacity { get; set; }

    }
}
