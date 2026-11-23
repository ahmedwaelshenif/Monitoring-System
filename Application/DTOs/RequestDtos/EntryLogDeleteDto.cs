using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class EntryLogDeleteDto
    {


        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? Cnt { get; set; }
    }
}
