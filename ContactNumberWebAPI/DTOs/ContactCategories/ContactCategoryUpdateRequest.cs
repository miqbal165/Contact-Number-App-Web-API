using System.ComponentModel.DataAnnotations;

namespace ContactNumberWebAPI.DTOs.ContactCategories;

public class ContactCategoryUpdateRequest
{
    public string Name { get; set; } = string.Empty;
}
