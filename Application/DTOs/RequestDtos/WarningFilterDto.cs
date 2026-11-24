using Domain.Enums;

namespace Application.DTOs.RequestDtos
{
    public class WarningFilterDto : PagedRequestDto
    {
        public int? LabId { get; set; }

        public string? LabName { get; set; }
        public Alert? Warning { get; set; }


    }
}