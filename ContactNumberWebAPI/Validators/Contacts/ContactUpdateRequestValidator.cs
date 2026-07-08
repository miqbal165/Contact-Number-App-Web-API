using ContactNumberWebAPI.DTOs.Contacts;
using FluentValidation;

namespace ContactNumberWebAPI.Validators.Contacts;

public sealed class ContactUpdateRequestValidator
    : AbstractValidator<ContactUpdateRequest>
{
    public ContactUpdateRequestValidator()
    {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Nama contact wajib diisi.")
            .MaximumLength(100)
            .WithMessage("Nama contact maksimal 100 karakter.");

        RuleFor(request => request.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Nomor telepon wajib diisi.")
            .MaximumLength(30)
            .WithMessage("Nomor telepon maksimal 30 karakter.")
            .Matches(@"^[0-9+\-\s()]+$")
            .WithMessage("Format nomor telepon tidak valid.");

        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(150)
            .WithMessage("Email maksimal 150 karakter.")
            .EmailAddress()
            .WithMessage("Format email tidak valid.")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.Address)
            .MaximumLength(250)
            .WithMessage("Alamat maksimal 250 karakter.")
            .When(request => !string.IsNullOrWhiteSpace(request.Address));

        RuleFor(request => request.ContactCategoryId)
            .NotEmpty()
            .WithMessage("Kategori contact wajib dipilih.");
    }
}