using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class EntryLogFilterDto : PagedRequestDto
    {
        public int? LabId { get; set; }

        public EntryStatus? Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }


    }
}
