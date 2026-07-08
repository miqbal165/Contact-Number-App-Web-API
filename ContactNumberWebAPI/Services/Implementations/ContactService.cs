using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.Contacts;
using ContactNumberWebAPI.Models;
using ContactNumberWebAPI.Repositories.Interfaces;
using ContactNumberWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactNumberWebAPI.Services.Implementations;

public class ContactService : IContactService
{
    private readonly IRepository<Contact> _contactRepository;
    private readonly IRepository<ContactCategory> _categoryRepository;
    private readonly IMapper _mapper;

    public ContactService(
        IRepository<Contact> contactRepository,
        IRepository<ContactCategory> categoryRepository,
        IMapper mapper)
    {
        _contactRepository = contactRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PagedResult<ContactResponse>>> GetAllAsync(
        string? search,
        Guid? categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        IQueryable<Contact> query = _contactRepository.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string keyword = search.Trim().ToLowerInvariant();

            query = query.Where(contact =>
                contact.Name.ToLower().Contains(keyword) ||
                contact.PhoneNumber.ToLower().Contains(keyword) ||
                (contact.Email != null && contact.Email.ToLower().Contains(keyword)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(contact => contact.ContactCategoryId == categoryId.Value);
        }

        int totalItems = await query.CountAsync(cancellationToken);

        List<ContactResponse> contacts = await query
            .OrderBy(contact => contact.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<ContactResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        PagedResult<ContactResponse> response = new()
        {
            Items = contacts,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return ServiceResult<PagedResult<ContactResponse>>.Ok(response);
    }

    public async Task<ServiceResult<ContactResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        Contact? contact = await _contactRepository.Query()
            .AsNoTracking()
            .Include(contact => contact.ContactCategory)
            .FirstOrDefaultAsync(contact => contact.Id == id, cancellationToken);

        if (contact is null)
        {
            return ServiceResult<ContactResponse>.Fail(
                "Contact tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        ContactResponse response = _mapper.Map<ContactResponse>(contact);

        return ServiceResult<ContactResponse>.Ok(response);
    }

    public async Task<ServiceResult<ContactResponse>> CreateAsync(
        ContactCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        bool categoryExists = await _categoryRepository.AnyAsync(
            category => category.Id == request.ContactCategoryId, cancellationToken);

        if (!categoryExists)
        {
            return ServiceResult<ContactResponse>.Fail(
                "Category tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        Contact contact = _mapper.Map<Contact>(request);

        contact.Name = contact.Name.Trim();
        contact.PhoneNumber = contact.PhoneNumber.Trim();
        contact.Email = string.IsNullOrWhiteSpace(contact.Email) ? null : contact.Email.Trim();
        contact.Address = string.IsNullOrWhiteSpace(contact.Address) ? null : contact.Address.Trim();

        await _contactRepository.AddAsync(contact, cancellationToken);

        await _contactRepository.SaveChangesAsync(cancellationToken);

        Contact createdContact = await _contactRepository.Query()
            .AsNoTracking()
            .Include(existingContact =>
                existingContact.ContactCategory)
            .FirstAsync(
                existingContact => existingContact.Id == contact.Id,
                cancellationToken);

        ContactResponse response = _mapper.Map<ContactResponse>(createdContact);

        return ServiceResult<ContactResponse>.Created(
            response,
            "Contact berhasil dibuat.");
    }

    public async Task<ServiceResult<ContactResponse>> UpdateAsync(
        Guid id,
        ContactUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        Contact? contact = await _contactRepository.Query()
            .FirstOrDefaultAsync(
                contact => contact.Id == id,
                cancellationToken);

        if (contact is null)
        {
            return ServiceResult<ContactResponse>.Fail(
                "Contact tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        bool categoryExists = await _categoryRepository.AnyAsync(
            category => category.Id == request.ContactCategoryId,
            cancellationToken);

        if (!categoryExists)
        {
            return ServiceResult<ContactResponse>.Fail(
                "Category tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        _mapper.Map(request, contact);
        contact.Name = contact.Name.Trim();
        contact.PhoneNumber = contact.PhoneNumber.Trim();
        contact.Email = string.IsNullOrWhiteSpace(contact.Email) ? null : contact.Email.Trim();
        contact.Address = string.IsNullOrWhiteSpace(contact.Address) ? null : contact.Address.Trim();

        _contactRepository.Update(contact);

        await _contactRepository.SaveChangesAsync(cancellationToken);

        Contact updatedContact = await _contactRepository.Query()
            .AsNoTracking()
            .Include(existingContact =>
                existingContact.ContactCategory)
            .FirstAsync(
                existingContact => existingContact.Id == id,
                cancellationToken);

        ContactResponse response = _mapper.Map<ContactResponse>(updatedContact);

        return ServiceResult<ContactResponse>.Ok(
            response,
            "Contact berhasil diubah.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        Contact? contact = await _contactRepository.Query()
            .FirstOrDefaultAsync(
                contact => contact.Id == id,
                cancellationToken);

        if (contact is null)
        {
            return ServiceResult<object>.Fail("Contact tidak ditemukan.", ServiceResultStatus.NotFound);
        }

        _contactRepository.Remove(contact);

        await _contactRepository.SaveChangesAsync(cancellationToken);

        return ServiceResult<object>.Ok(null!, "Contact berhasil dihapus.");
    }
}
