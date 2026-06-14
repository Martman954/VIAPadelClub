using System.Reflection;
using VIAPadelClub.Core.Application.AppEntry;
using VIAPadelClub.Core.QueryContracts;
using VIAPadelClub.Core.Tools.ObjectMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/hello", () => "Hello World!");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();