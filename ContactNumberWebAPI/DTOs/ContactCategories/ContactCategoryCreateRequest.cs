using System.ComponentModel.DataAnnotations;

namespace ContactNumberWebAPI.DTOs.ContactCategories;

public class ContactCategoryCreateRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; }
}