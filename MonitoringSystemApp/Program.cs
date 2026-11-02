using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
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


//Repositories

builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

builder.Services.AddScoped<
    IRegistrationRequestRepository,
    RegistrationRequestRepository>();

builder.Services.AddScoped<
    IMemberRepository,
    MemberRepository>();

builder.Services.AddScoped<
    IDashboardAccountRepository,
    DashboardAccountRepository>();

builder.Services.AddScoped<
    IUnitOfWork,
    UnitOfWork>();


//Services

builder.Services.AddScoped<
    IRegistrationRequestService,
    RegistrationRequestService>();

builder.Services.AddScoped<
    IMemberService,
    MemberService>();

builder.Services.AddScoped<
    IDashboardAccountService,
    DashboardAccountService>();


builder.Services.AddScoped<IPasswordHasher<DashboardAccount>,
    PasswordHasher<DashboardAccount>>();



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
