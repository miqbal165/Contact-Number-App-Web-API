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
    private readonly IRepository<ContactCategory> _contactCategoryRepository;
    private readonly IMapper _mapper;

    public ContactService(
        IRepository<Contact> contactRepository,
        IRepository<ContactCategory> contactCategoryRepository,
        IMapper mapper)
    {
        _contactRepository = contactRepository;
        _contactCategoryRepository = contactCategoryRepository;
        _mapper = mapper;
    }
    
    public async Task<ServiceResult<PagedResult<ContactResponse>>> GetAllAsync(
        string? search,
        Guid? categoryId,
        int page,
        int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        IQueryable<Contact> query = _contactRepository.Query()
            .AsNoTracking()
            .Include(contact => contact.ContactCategory);

        if (!string.IsNullOrWhiteSpace(search))
        {
            string keyword = search.Trim().ToLower();
            query = query.Where(contact =>
                contact.Name.ToLower().Contains(keyword) ||
                contact.PhoneNumber.ToLower().Contains(keyword) ||
                (contact.Email != null && contact.Email.ToLower().Contains(keyword)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(contact => contact.ContactCategoryId == categoryId.Value);
        }

        int totalItems = await query.CountAsync();

        List<ContactResponse> contacts = await query
            .OrderBy(contact => contact.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<ContactResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        PagedResult<ContactResponse> response = new PagedResult<ContactResponse>
        {
            Items = contacts,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return ServiceResult<PagedResult<ContactResponse>>.Ok(response);
    }

    public async Task<ServiceResult<ContactResponse>> GetByIdAsync(Guid id)
    {
        Contact? contact = await _contactRepository.Query()
            .AsNoTracking()
            .Include(contact => contact.ContactCategory)
            .FirstOrDefaultAsync(contact => contact.Id == id);

        if (contact is null)
        {
            return ServiceResult<ContactResponse>.Fail(
                "Contact tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        ContactResponse response = _mapper.Map<ContactResponse>(contact);
        return ServiceResult<ContactResponse>.Ok(response);
    }

    public async Task<ServiceResult<ContactResponse>> CreateAsync(ContactCreateRequest request)
    {
        bool categoryExists = await _contactCategoryRepository.AnyAsync(
            category => category.Id == request.ContactCategoryId);

        if (!categoryExists)
        {
            return ServiceResult<ContactResponse>.Fail(
                "Category tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        Contact contact = _mapper.Map<Contact>(request);
        contact.Name = request.Name.Trim();
        contact.PhoneNumber = request.PhoneNumber.Trim();
        contact.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        contact.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        await _contactRepository.AddAsync(contact);
        await _contactRepository.SaveChangesAsync();

        Contact createdContact = await _contactRepository.Query()
            .AsNoTracking()
            .Include(existingContact => existingContact.ContactCategory)
            .FirstAsync(existingContact => existingContact.Id == contact.Id);

        ContactResponse response = _mapper.Map<ContactResponse>(createdContact);
        return ServiceResult<ContactResponse>.Created(response, "Contact berhasil dibuat.");
    }

    public async Task<ServiceResult<ContactResponse>> UpdateAsync(Guid id, ContactUpdateRequest request)
    {
        Contact? contact = await _contactRepository.Query()
            .FirstOrDefaultAsync(contact => contact.Id == id);

        if (contact is null)
        {
            return ServiceResult<ContactResponse>.Fail(
                "Contact tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        bool categoryExists = await _contactCategoryRepository.AnyAsync(
            category => category.Id == request.ContactCategoryId);

        if (!categoryExists)
        {
            return ServiceResult<ContactResponse>.Fail(
                "Category tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        contact.Name = request.Name.Trim();
        contact.PhoneNumber = request.PhoneNumber.Trim();
        contact.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        contact.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        contact.ContactCategoryId = request.ContactCategoryId;

        _contactRepository.Update(contact);
        await _contactRepository.SaveChangesAsync();

        Contact updatedContact = await _contactRepository.Query()
            .AsNoTracking()
            .Include(existingContact => existingContact.ContactCategory)
            .FirstAsync(existingContact => existingContact.Id == id);

        ContactResponse response = _mapper.Map<ContactResponse>(updatedContact);
        return ServiceResult<ContactResponse>.Ok(response, "Contact berhasil diubah.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(Guid id)
    {
        Contact? contact = await _contactRepository.Query()
            .FirstOrDefaultAsync(contact => contact.Id == id);

        if (contact is null)
        {
            return ServiceResult<object>.Fail(
                "Contact tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        _contactRepository.Remove(contact);
        await _contactRepository.SaveChangesAsync();

        return ServiceResult<object>.Ok(null!, "Contact berhasil dihapus.");
    }
}