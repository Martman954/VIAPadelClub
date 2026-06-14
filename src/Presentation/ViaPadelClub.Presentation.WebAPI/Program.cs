using VIAPadelClub.Core.Tools.ObjectMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI();
    app.UseSwagger();
}
builder.Services.AddSingleton<IObjectMapper, ObjectMapper>();
//builder.Services.AddSingleton<IMapping, GuestToGuestDtoMapping>();

app.MapGet("/hello", () => "Hello World!");   // <-- test endpoint

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();