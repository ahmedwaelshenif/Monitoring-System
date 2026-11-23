using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDtos
{
    public class LabResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }

        public bool active { get; set; }
    }
}
