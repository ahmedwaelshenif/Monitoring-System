using Domain.Enums;

namespace Application.DTOs.RequestDtos
{
    public class WarningFilterDto
    {
        public int? LabId { get; set; }

        public string? LabName { get; set; }
        public Alert? Warning { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}