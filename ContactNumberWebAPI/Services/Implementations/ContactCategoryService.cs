using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.ContactCategories;
using ContactNumberWebAPI.Models;
using ContactNumberWebAPI.Repositories.Interfaces;
using ContactNumberWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactNumberWebAPI.Services.Implementations;

public class ContactCategoryService : IContactCategoryService
{
    private readonly IRepository<ContactCategory> _contactCategoryRepository;
    private readonly IRepository<Contact> _contactRepository;
    
    public ContactCategoryService(
        IRepository<ContactCategory> contactCategoryRepository, 
        IRepository<Contact> contactRepository)
    {
        _contactCategoryRepository = contactCategoryRepository;
        _contactRepository = contactRepository;
    }


    public async Task<ServiceResult<IReadOnlyList<ContactCategoryResponse>>> GetAllAsync()
    {
        List<ContactCategory> categories = await _contactCategoryRepository.Query()
            .AsNoTracking()
            .OrderBy(contactCategory => contactCategory.Name)
            .ToListAsync();

        return null;
    }

    public Task<ServiceResult<ContactCategoryResponse>> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<ContactCategoryResponse>> CreateAsync(ContactCategoryCreateRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<ContactCategoryResponse>> UpdateAsync(Guid id, ContactCategoryUpdateRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<object>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}