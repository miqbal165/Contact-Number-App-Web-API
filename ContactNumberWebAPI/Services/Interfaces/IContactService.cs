using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.Contacts;

namespace ContactNumberWebAPI.Services.Interfaces;

public interface IContactService
{
    Task<ServiceResult<PagedResult<ContactResponse>>> GetAllAsync(string? search, Guid? categoryId, int page, int pageSize);
    Task<ServiceResult<ContactResponse>> GetByIdAsync(Guid id);
    Task<ServiceResult<ContactResponse>> CreateAsync(ContactCreateRequest request);
    Task<ServiceResult<ContactResponse>> UpdateAsync(Guid id, ContactUpdateRequest request);
    Task<ServiceResult<object>> DeleteAsync(Guid id);
}