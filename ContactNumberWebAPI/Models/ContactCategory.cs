namespace ContactNumberWebAPI.Models;

public class ContactCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<Contact> Contacts { get; set; } = [];
}