using System.Text;
using System.Text.Json.Serialization;
using Dobu.Api.Extensions;
using Dobu.Api.Health;
using Dobu.Api.Middleware;
using Dobu.Api.Observability;
using Dobu.Application.Services;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/dobu-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddScoped<IAuthService>(services =>
    new AuthService(services.GetRequiredService<IUserStore>(), builder.Configuration["Jwt:Key"]!));

var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ValidateIssuer = false, ValidateAudience = false, ValidateLifetime = true
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("ExternalServiceHealthCheck", client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddHealthChecks()
    .AddDbContextCheck<DobuDbContext>("database", tags: ["ready"])
    .AddCheck<ExternalServiceHealthCheck>("external-service", tags: ["ready"]);
builder.Services.AddDobuObservability();

var app = builder.Build();
if ((builder.Configuration["Database:Provider"] ?? "Sqlite").Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<DobuDbContext>().Database.EnsureCreated();
}
app.UseSerilogRequestLogging();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestMetricsMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dobu API v1"); options.RoutePrefix = "swagger"; });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var payload = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.ToDictionary(entry => entry.Key, entry => new { status = entry.Value.Status.ToString(), description = entry.Value.Description })
        };
        await context.Response.WriteAsJsonAsync(payload);
    }
});
app.MapPrometheusScrapingEndpoint("/metrics");
app.MapControllers();
app.Run();

public partial class Program { }
