using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.ContactCategories;

namespace ContactNumberWebAPI.Services.Interfaces;

public interface IContactCategoryService
{
    Task<ServiceResult<IReadOnlyList<ContactCategoryResponse>>> GetAllAsync();
    Task<ServiceResult<ContactCategoryResponse>> GetByIdAsync(Guid id);
    Task<ServiceResult<ContactCategoryResponse>> CreateAsync(ContactCategoryCreateRequest request);
    Task<ServiceResult<ContactCategoryResponse>> UpdateAsync(Guid id, ContactCategoryUpdateRequest request);
    Task<ServiceResult<object>> DeleteAsync(Guid id);
}