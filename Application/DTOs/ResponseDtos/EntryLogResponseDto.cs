using Domain.Enums;

namespace Application.DTOs.ResponseDtos
{
    public class EntryLogResponseDto
    {
        public int Id { get; set; }

        public int? VisitorId { get; set; }
        public int? MemberId { get; set; }
        public string? VisitorName { get; set; }

        public int? LabId { get; set; }
        public string? LabName { get; set; }

        public DateTime EventTime { get; set; }

        public EntryStatus Status { get; set; }

        public byte[]? FaceImage { get; set; }
    }
}