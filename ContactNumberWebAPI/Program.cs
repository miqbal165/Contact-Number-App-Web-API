using ContactNumberWebAPI.Extensions;
using ContactNumberWebAPI.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

ApplicationConfiguration.AddApplicationServices(
    builder.Services,
    builder.Configuration);

AuthenticationConfiguration.AddJwtAuthentication(
    builder.Services,
    builder.Configuration);

SwaggerConfiguration.AddSwagger(
    builder.Services);

WebApplication app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
