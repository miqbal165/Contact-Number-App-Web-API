using ContactNumberWebAPI.Data;
using ContactNumberWebAPI.DTOs.Auth;
using ContactNumberWebAPI.DTOs.ContactCategories;
using ContactNumberWebAPI.DTOs.Contacts;
using ContactNumberWebAPI.Helpers;
using ContactNumberWebAPI.Repositories.Implementations;
using ContactNumberWebAPI.Repositories.Interfaces;
using ContactNumberWebAPI.Services.Implementations;
using ContactNumberWebAPI.Services.Interfaces;
using ContactNumberWebAPI.Settings;
using ContactNumberWebAPI.Validators.Auth;
using ContactNumberWebAPI.Validators.ContactCategories;
using ContactNumberWebAPI.Validators.Contacts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ContactNumberWebAPI.Extensions;

public static class ApplicationConfiguration
{
    public static void AddApplicationServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' belum dikonfigurasi.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.Configure<JwtSettings>(
            configuration.GetRequiredSection("Jwt"));

        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<JwtTokenGenerator>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IContactCategoryService, ContactCategoryService>();
        services.AddScoped<IContactService, ContactService>();

        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<ContactCategoryCreateRequest>, ContactCategoryCreateRequestValidator>();
        services.AddScoped<IValidator<ContactCategoryUpdateRequest>, ContactCategoryUpdateRequestValidator>();
        services.AddScoped<IValidator<ContactCreateRequest>, ContactCreateRequestValidator>();
        services.AddScoped<IValidator<ContactUpdateRequest>, ContactUpdateRequestValidator>();
    }
}
