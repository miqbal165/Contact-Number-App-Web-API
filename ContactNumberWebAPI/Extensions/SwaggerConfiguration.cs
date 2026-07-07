using Microsoft.OpenApi.Models;

namespace ContactNumberWebAPI.Extensions;

public class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerWithJwt(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Secure Contact API",
                Version = "v1"
            });

            OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Masukkan token dengan format: Bearer {token}",
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

            options.AddSecurityDefinition("Bearer", securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    securityScheme,
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}