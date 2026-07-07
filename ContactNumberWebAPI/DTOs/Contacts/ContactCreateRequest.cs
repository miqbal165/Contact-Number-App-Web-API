using System.ComponentModel.DataAnnotations;

namespace ContactNumberWebAPI.DTOs.Contacts;

public class ContactCreateRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress, MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(250)]
    public string? Address { get; set; }

    [Required]
    public Guid ContactCategoryId { get; set; }
}