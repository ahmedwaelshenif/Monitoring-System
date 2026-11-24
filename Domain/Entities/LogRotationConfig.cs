public class LogRotationConfig
{
    public int Id { get; set; } = 1; // Single row table
    public bool IsEnabled { get; set; } = false; // Admin can toggle On/Off
    public int RetentionDays { get; set; } = 90;  // Delete logs older than X days
    public int RunAtHourUtc { get; set; } = 2;   // Hour to run (0 - 23, e.g. 2 = 2:00 AM UTC)
    public DateTime? LastRunAt { get; set; }     // Prevents running twice on the same day
}