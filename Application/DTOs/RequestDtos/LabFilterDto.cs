using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Application.DTOs.RequestDtos
{
    public class LabFilterDto : PagedRequestDto
    {
       
        public int ?Id { get; set; }

        public string ?Name { get; set; }

        public bool ?Active { get; set; } = true;



    }
}
