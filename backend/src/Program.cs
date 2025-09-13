using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog basic bootstrap (T004 to be expanded later)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.WithProcessId()
    .Enrich.WithEnvironmentName()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.MapControllers();

app.Run();