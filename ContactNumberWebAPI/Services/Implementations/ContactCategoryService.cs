using AutoMapper;
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
    private readonly IMapper _mapper;
    
    public ContactCategoryService(
        IRepository<ContactCategory> contactCategoryRepository, 
        IRepository<Contact> contactRepository,
        IMapper mapper)
    {
        _contactCategoryRepository = contactCategoryRepository;
        _contactRepository = contactRepository;
        _mapper = mapper;
    }


    public async Task<ServiceResult<IReadOnlyList<ContactCategoryResponse>>> GetAllAsync()
    {
        List<ContactCategory> categories = await _contactCategoryRepository.Query()
            .AsNoTracking()
            .OrderBy(contactCategory => contactCategory.Name)
            .ToListAsync();

        IReadOnlyList<ContactCategoryResponse> response = _mapper
            .Map<IReadOnlyList<ContactCategoryResponse>>(categories);
        
        
        return ServiceResult<IReadOnlyList<ContactCategoryResponse>>.Ok(response);
    }

    public async Task<ServiceResult<ContactCategoryResponse>> GetByIdAsync(Guid id)
    {
        ContactCategory? category = await _contactCategoryRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(category => category.Id == id);

        if (category is null)
        {
            return ServiceResult<ContactCategoryResponse>.Fail(
                "Category tidak ditemukan",
                ServiceResultStatus.NotFound
            );
        }

        ContactCategoryResponse response = _mapper.Map<ContactCategoryResponse>(category);
        return ServiceResult<ContactCategoryResponse>.Ok(response);
    }

    public async Task<ServiceResult<ContactCategoryResponse>> CreateAsync(ContactCategoryCreateRequest request)
    {
        string name = request.Name.Trim();

        bool nameExists = await _contactCategoryRepository.AnyAsync(category => 
            category.Name.ToLower() == name.ToLower());

        if (nameExists)
        {
            return ServiceResult<ContactCategoryResponse>.Fail(
                "Nama category sudah digunakan",
                ServiceResultStatus.Conflict
            );
        }
        
        ContactCategory category = _mapper.Map<ContactCategory>(request);
        category.Name = name;

        await _contactCategoryRepository.AddAsync(category);
        await _contactCategoryRepository.SaveChangesAsync();

        ContactCategoryResponse response = _mapper.Map<ContactCategoryResponse>(category);
        return ServiceResult<ContactCategoryResponse>.Created(response, "Category berhasil dibuat.");
    }

    public async Task<ServiceResult<ContactCategoryResponse>> UpdateAsync(Guid id, ContactCategoryUpdateRequest request)
    {
        ContactCategory? category = await _contactCategoryRepository.Query()
            .FirstOrDefaultAsync(category => category.Id == id);

        if (category is null)
        {
            return ServiceResult<ContactCategoryResponse>.Fail(
                "Category tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        string name = request.Name.Trim();

        bool nameExists = await _contactCategoryRepository.AnyAsync(existingCategory =>
            existingCategory.Id != id &&
            existingCategory.Name.ToLower() == name.ToLower());

        if (nameExists)
        {
            return ServiceResult<ContactCategoryResponse>.Fail(
                "Nama category sudah digunakan.",
                ServiceResultStatus.Conflict);
        }

        category.Name = name;
        _contactCategoryRepository.Update(category);
        await _contactCategoryRepository.SaveChangesAsync();

        ContactCategoryResponse response = _mapper.Map<ContactCategoryResponse>(category);
        return ServiceResult<ContactCategoryResponse>.Ok(response, "Category berhasil diubah.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(Guid id)
    {
        ContactCategory? category = await _contactCategoryRepository.Query()
            .FirstOrDefaultAsync(category => category.Id == id);

        if (category is null)
        {
            return ServiceResult<object>.Fail(
                "Category tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }
        
        bool hasContacts = await _contactRepository.AnyAsync(contact => contact.ContactCategoryId == id);

        if (hasContacts)
        {
            return ServiceResult<object>.Fail(
                "Category tidak bisa dihapus karena masih digunakan contact.",
                ServiceResultStatus.Conflict);
        }
        
        _contactCategoryRepository.Remove(category);
        await _contactCategoryRepository.SaveChangesAsync();

        return ServiceResult<object>.Ok(null!, "Category berhasil dihapus.");
    }
}