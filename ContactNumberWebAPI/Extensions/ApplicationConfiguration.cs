using ContactNumberWebAPI.Data;
using ContactNumberWebAPI.Helpers;
using ContactNumberWebAPI.Repositories.Implementations;
using ContactNumberWebAPI.Repositories.Interfaces;
using ContactNumberWebAPI.Services.Implementations;
using ContactNumberWebAPI.Services.Interfaces;
using ContactNumberWebAPI.Settings;
using Microsoft.EntityFrameworkCore;

namespace ContactNumberWebAPI.Extensions;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplicationServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<JwtTokenGenerator>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IContactCategoryService, ContactCategoryService>();
        services.AddScoped<IContactService, ContactService>();

        return services;
    }
}