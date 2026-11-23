using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Application.DTOs.RequestDtos
{
    public class SearchLabRequestDto
    {
       
        public int ?Id { get; set; }

        public string ?Name { get; set; }

        public bool ?Active { get; set; } = true;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}
