using Serilog;

builder.Host.UseSerilog();


var builder = WebApplication.CreateBuilder(args);

// Serilog basic bootstrap (T004 to be expanded later)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.WithProcessId()
    .Enrich.WithEnvironmentName()
    .CreateLogger();

builder.Host.UseSerilog();

// Register application services for DI
builder.Services.AddScoped<AccessControl.Api.Services.AccessDecisionService>();
builder.Services.AddScoped<AccessControl.Api.Services.ProfileService>();
builder.Services.AddScoped<AccessControl.Api.Services.CredentialService>();
builder.Services.AddScoped<AccessControl.Api.Services.ZonePermissionService>();
builder.Services.AddScoped<AccessControl.Api.Services.TemplateService>();
builder.Services.AddScoped<AccessControl.Api.Services.ReportingService>();
builder.Services.AddScoped<AccessControl.Api.Services.VisitorBadgeService>();
builder.Services.AddScoped<AccessControl.Api.Services.ReasonCodeService>();
builder.Services.AddScoped<AccessControl.Api.RealTime.RealTimeEventPublisher>();

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