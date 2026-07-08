using ContactNumberWebAPI.DTOs.ContactCategories;
using FluentValidation;

namespace ContactNumberWebAPI.Validators.ContactCategories;

public sealed class ContactCategoryUpdateRequestValidator
    : AbstractValidator<ContactCategoryUpdateRequest>
{
    public ContactCategoryUpdateRequestValidator()
    {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Nama kategori wajib diisi.")
            .MaximumLength(100)
            .WithMessage("Nama kategori maksimal 100 karakter.");
    }
}