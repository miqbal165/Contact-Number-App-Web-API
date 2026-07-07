using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.ContactCategories;
using ContactNumberWebAPI.Models;
using ContactNumberWebAPI.Repositories.Interfaces;
using ContactNumberWebAPI.Services.Interfaces;

namespace ContactNumberWebAPI.Services.Implementations;

public class ContactCategoryService : IContactCategoryService
{
    private readonly IRepository<ContactCategory> _contactCategoryRepository;

    public ContactCategoryService(IRepository<ContactCategory> contactCategoryRepository)
    {
        _contactCategoryRepository = contactCategoryRepository;
    }


    public Task<ServiceResult<IReadOnlyList<ContactCategoryResponse>>> GetAllAsync()
    {
        throw new NotImplementedException();
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