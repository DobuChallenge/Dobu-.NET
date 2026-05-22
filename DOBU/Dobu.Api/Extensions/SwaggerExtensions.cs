using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Dobu.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Dobu API",
                Version = "v1",
                Description = "API REST para gerenciamento veterinario inteligente de usuarios, pets, consultas, agendamentos, vacinas, pagamentos, DobuCam e analises IA.",
                Contact = new OpenApiContact
                {
                    Name = "Equipe Dobu",
                    Email = "contato@dobu.com"
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });
    }
}
