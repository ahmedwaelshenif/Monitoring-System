using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonitoringSystemApp.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpClient<IEmbeddingService, EmbeddingService>(client =>
{
    client.BaseAddress = new Uri("http://127.0.0.1:8000/");
});


// Repositories

builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

builder.Services.AddScoped<
    IUnitOfWork,
    UnitOfWork>();

// Services

builder.Services.AddScoped<
    IRegistrationRequestService,
    RegistrationRequestService>();

builder.Services.AddScoped<
    IVisitorService,
    VisitorService>();

builder.Services.AddScoped<
    IDashboardAccountService,
    DashboardAccountService>();

builder.Services.AddScoped<
    IEntryLogService,
    EntryLogService>();

builder.Services.AddScoped<
    ILabService,
    LabService>();

builder.Services.AddScoped<
    IWarningService,
    WarningService>();

builder.Services.AddScoped<
    IEmailService,
    EmailService>();



builder.Services.AddScoped<IPasswordHasher<DashboardAccount>,
    PasswordHasher<DashboardAccount>>();

//Back ground worker
builder.Services.AddHostedService<
    LogRotationBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment() && builder.Configuration.GetValue("SeedData:Enabled", true))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<DashboardAccount>>();
    var demoPassword = builder.Configuration["SeedData:DemoPassword"] ?? "DevSeed!Only2026";
    await DevelopmentDataSeeder.SeedAsync(db, passwordHasher, demoPassword);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var password = builder.Configuration["EmailSettings:Password"];

Console.WriteLine(
    string.IsNullOrEmpty(password)
        ? "Email password not configured"
        : "Email password loaded successfully"
);

app.Run();
