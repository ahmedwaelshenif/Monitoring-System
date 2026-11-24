using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class LogRotationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LogRotationBackgroundService> _logger;

    public LogRotationBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<LogRotationBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Periodic check every 30 minutes
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var config = (await unitOfWork.Repository<LogRotationConfig>().GetAllAsync()).FirstOrDefault();

                // 1. Check if enabled by Admin
                if (config == null || !config.IsEnabled)
                    continue;

                var now = DateTime.UtcNow;

                // 2. Check if already ran today
                if (config.LastRunAt.HasValue && config.LastRunAt.Value.Date == now.Date)
                    continue;

                // 3. Check if current hour has reached the admin's chosen hour
                if (now.Hour >= config.RunAtHourUtc)
                {
                    _logger.LogInformation("Starting scheduled log rotation cleanup...");

                    var cutoffDate = now.AddDays(-config.RetentionDays);
                    var oldLogs = await unitOfWork.Repository<EntryLog>().GetAllAsync(l => l.EventTime < cutoffDate);

                    if (oldLogs.Any())
                    {
                        unitOfWork.Repository<EntryLog>().DeleteRange(oldLogs);
                        _logger.LogInformation("Deleted {Count} expired entry logs.", oldLogs.Count());
                    }

                    config.LastRunAt = now;
                    unitOfWork.Repository<LogRotationConfig>().Update(config);
                    await unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during automated log rotation.");
            }
        }
    }
}