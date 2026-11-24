using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class AccountFilterDto : PagedRequestDto
    {
        public string ? Email {get;set;}
        public string ? Name {get;set;}
        public Role ? role { get;set; }
        public bool ? IsActive { get; set; }


    }
}
