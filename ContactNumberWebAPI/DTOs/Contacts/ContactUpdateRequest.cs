
namespace ContactNumberWebAPI.DTOs.Contacts;

public class ContactUpdateRequest
{
    public string Name { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public Guid ContactCategoryId { get; set; }
}
