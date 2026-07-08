using System.ComponentModel.DataAnnotations;

namespace ContactNumberWebAPI.DTOs.ContactCategories;

public class ContactCategoryCreateRequest
{
    public string Name { get; set; } = string.Empty;
}
