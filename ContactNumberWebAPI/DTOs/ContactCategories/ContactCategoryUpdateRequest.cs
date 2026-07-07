using System.ComponentModel.DataAnnotations;

namespace ContactNumberWebAPI.DTOs.ContactCategories;

public class ContactCategoryUpdateRequest
{
    [Required, MaxLength(100)] 
    public string Name { get; set; } = string.Empty;
}