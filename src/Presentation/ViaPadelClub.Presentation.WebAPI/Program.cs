using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.Application.ExternalServices;
using VIAPadelClub.Core.Domain.Contracts.Courts;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Domain.Contracts.Schedules;
using VIAPadelClub.Core.Domain.Repositories;
using VIAPadelClub.Core.Domain.UnitOfWork;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Core.Tools.ObjectMapper;
using VIAPadelClub.Infrastructure.EfcDomainPersistence;
using VIAPadelClub.Infrastructure.EfcDomainPersistence.Repositories;
using ViaPadelClub.Presentation.WebAPI.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var domainConnectionString = builder.Configuration.GetConnectionString("DomainModel")
    ?? "Data Source=VIAPadelClub.DomainModel.db";

builder.Services.AddDbContext<DomainModelContext>(options => options.UseSqlite(domainConnectionString));

builder.Services.AddScoped<IScheduleRepository, ScheduleRepositoryEfc>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepositoryEfc>();
builder.Services.AddScoped<ICourtRepository, CourtRepositoryEfc>();

builder.Services.AddScoped<IBookingCourtFinder, EfBookingCourtFinder>();
builder.Services.AddScoped<ICourtHasBookingChecker, EfCourtHasBookingChecker>();
builder.Services.AddScoped<IScheduleDateConflictChecker, EfScheduleDateConflictChecker>();
builder.Services.AddScoped<IEmailInUseChecker, EfEmailInUseChecker>();

builder.Services.AddScoped<ICourtRemovalNotifier, NoOpCourtRemovalNotifier>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// --- Object mapper + mapping registrations from this assembly ---
var mappingRegistrations = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => t is { IsClass: true, IsAbstract: false }
                && typeof(IMapping).IsAssignableFrom(t));

foreach (var mapping in mappingRegistrations)
    builder.Services.AddSingleton(typeof(IMapping), mapping);

builder.Services.AddSingleton<IObjectMapper, ObjectMapper>();

builder.Services.AddApplicationCommandDispatch();
builder.Services.AddApplicationQueryDispatch(
    typeof(VIAPadelClub.Infrastructure.EfcQueries.DependencyInjectionExtensions).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DomainModelContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok(new
{
    Name = "VIAPadelClub API",
    Docs = "/swagger",
    Health = "/hello"
}));

app.MapGet("/hello", () => "Hello World!");

// Keep HTTPS redirection outside Development so local http profile works without redirect warnings.
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();