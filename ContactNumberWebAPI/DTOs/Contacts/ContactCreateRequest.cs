using System.ComponentModel.DataAnnotations;

namespace ContactNumberWebAPI.DTOs.Contacts;

public class ContactCreateRequest
{
    public string Name { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public Guid ContactCategoryId { get; set; }
}
