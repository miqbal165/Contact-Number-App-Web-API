using ContactNumberWebAPI.DTOs.Auth;
using FluentValidation;

namespace ContactNumberWebAPI.Validators.Auth;

public sealed class LoginRequestValidator
    : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email wajib diisi.")
            .EmailAddress()
            .WithMessage("Format email tidak valid.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("Password wajib diisi.");
    }
}