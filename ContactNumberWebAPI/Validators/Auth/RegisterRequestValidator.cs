using ContactNumberWebAPI.DTOs.Auth;
using FluentValidation;

namespace ContactNumberWebAPI.Validators.Auth;

public sealed class RegisterRequestValidator
    : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Nama lengkap wajib diisi.")
            .MaximumLength(100)
            .WithMessage("Nama lengkap maksimal 100 karakter.");

        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email wajib diisi.")
            .MaximumLength(150)
            .WithMessage("Email maksimal 150 karakter.")
            .EmailAddress()
            .WithMessage("Format email tidak valid.");

        RuleFor(request => request.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Password wajib diisi.")
            .MinimumLength(8)
            .WithMessage("Password minimal 8 karakter.")
            .MaximumLength(100)
            .WithMessage("Password maksimal 100 karakter.");
    }
}