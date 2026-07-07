using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.Contacts;
using ContactNumberWebAPI.Models;
using ContactNumberWebAPI.Repositories.Interfaces;
using ContactNumberWebAPI.Services.Interfaces;

namespace ContactNumberWebAPI.Services.Implementations;

public class ContactService : IContactService
{
    private readonly IRepository<Contact> _contactRepository;

    public ContactService(IRepository<Contact> contactRepository)
    {
        _contactRepository = contactRepository;
    }
    
    public Task<ServiceResult<PagedResult<ContactResponse>>> GetAllAsync(string? search, Guid? categoryId, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<ContactResponse>> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<ContactResponse>> CreateAsync(ContactCreateRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<ContactResponse>> UpdateAsync(Guid id, ContactUpdateRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<object>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}