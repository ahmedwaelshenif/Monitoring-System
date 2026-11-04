using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDtos
{
    public class SearchAccount
    {
        public string ? Email {get;set;}
        public string ? Name {get;set;}
        public Role ? role { get;set; }
        public bool ? status { get; set; }
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

    }
}
