public class LogRotationConfigDto
{
    public bool IsEnabled { get; set; }
    public int RetentionDays { get; set; } = 90;
    public int RunAtHourUtc { get; set; } = 2; // 0 to 23
    public DateTime? LastRunAt { get; set; }
}