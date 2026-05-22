using Dobu.Api.Extensions;
using System.Text.Json.Serialization;
 
var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerDocumentation();
 
builder.Services.AddPersistence(builder.Configuration);
 
var app = builder.Build();
 
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dobu API v1");
    options.RoutePrefix = "swagger";
});
 
app.UseHttpsRedirection();
 
app.UseAuthorization();
 
app.MapControllers();
 
app.Run();
