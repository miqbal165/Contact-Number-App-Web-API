using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.Contacts;

namespace ContactNumberWebAPI.Services.Interfaces;

public interface IContactService
{
    Task<ServiceResult<PagedResult<ContactResponse>>> GetAllAsync(
        string? search,
        Guid? categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<ContactResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<ContactResponse>> CreateAsync(
        ContactCreateRequest request, 
        CancellationToken cancellationToken = default);

    Task<ServiceResult<ContactResponse>> UpdateAsync(
        Guid id,
        ContactUpdateRequest request,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<object>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
