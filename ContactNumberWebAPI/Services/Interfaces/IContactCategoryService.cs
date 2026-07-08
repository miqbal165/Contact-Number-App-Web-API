using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.ContactCategories;

namespace ContactNumberWebAPI.Services.Interfaces;

public interface IContactCategoryService
{
    Task<ServiceResult<IReadOnlyList<ContactCategoryResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ServiceResult<ContactCategoryResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<ContactCategoryResponse>> CreateAsync(
        ContactCategoryCreateRequest request,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<ContactCategoryResponse>> UpdateAsync(
        Guid id,
        ContactCategoryUpdateRequest request,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<object>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
