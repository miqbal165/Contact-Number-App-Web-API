using Microsoft.OpenApi.Models;

namespace ContactNumberWebAPI.Extensions;

public class SwaggerConfiguration
{
    public static void AddSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Contact Number API",
                Version = "v1"
            });

            OpenApiSecurityScheme bearerScheme =
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description =
                        "Masukkan access token JWT tanpa kata 'Bearer'.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

            options.AddSecurityDefinition(
                "Bearer",
                bearerScheme);

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        bearerScheme,
                        Array.Empty<string>()
                    }
                });
        });
    }
}
