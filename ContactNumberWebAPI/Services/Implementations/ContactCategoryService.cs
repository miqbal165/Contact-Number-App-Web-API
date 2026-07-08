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
    private readonly IRepository<ContactCategory> _categoryRepository;
    private readonly IRepository<Contact> _contactRepository;
    private readonly IMapper _mapper;

    public ContactCategoryService(
        IRepository<ContactCategory> categoryRepository,
        IRepository<Contact> contactRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _contactRepository = contactRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<ContactCategoryResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        List<ContactCategory> categories =
            await _categoryRepository.Query()
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .ToListAsync(cancellationToken);

        IReadOnlyList<ContactCategoryResponse> response = _mapper
            .Map<IReadOnlyList<ContactCategoryResponse>>(categories);

        return ServiceResult<IReadOnlyList<ContactCategoryResponse>>.Ok(response);
    }

    public async Task<ServiceResult<ContactCategoryResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        ContactCategory? category =
            await _categoryRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

        if (category is null)
        {
            return ServiceResult<ContactCategoryResponse>.Fail(
                "Category tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        ContactCategoryResponse response = _mapper
            .Map<ContactCategoryResponse>(category);

        return ServiceResult<ContactCategoryResponse>.Ok(response);
    }

    public async Task<ServiceResult<ContactCategoryResponse>> CreateAsync(
        ContactCategoryCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        string name = request.Name.Trim();
        string normalizedName = name.ToLowerInvariant();

        bool nameExists = await _categoryRepository.AnyAsync(
            category => category.Name.ToLower() == normalizedName,
            cancellationToken);

        if (nameExists)
        {
            return ServiceResult<ContactCategoryResponse>.Fail(
                "Nama category sudah digunakan.",
                ServiceResultStatus.Conflict);
        }

        ContactCategory category = _mapper.Map<ContactCategory>(request);

        category.Name = name;

        await _categoryRepository.AddAsync(category, cancellationToken);

        await _categoryRepository.SaveChangesAsync(cancellationToken);

        ContactCategoryResponse response = _mapper.Map<ContactCategoryResponse>(category);

        return ServiceResult<ContactCategoryResponse>.Created(
            response,
            "Category berhasil dibuat.");
    }

    public async Task<ServiceResult<ContactCategoryResponse>> UpdateAsync(
        Guid id,
        ContactCategoryUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        ContactCategory? category =
            await _categoryRepository.Query()
                .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

        if (category is null)
        {
            return ServiceResult<ContactCategoryResponse>.Fail(
                "Category tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        string name = request.Name.Trim();
        string normalizedName = name.ToLowerInvariant();

        bool nameExists = await _categoryRepository.AnyAsync(
            existingCategory =>
                existingCategory.Id != id &&
                existingCategory.Name.ToLower() == normalizedName,
            cancellationToken);

        if (nameExists)
        {
            return ServiceResult<ContactCategoryResponse>.Fail(
                "Nama category sudah digunakan.",
                ServiceResultStatus.Conflict);
        }

        _mapper.Map(request, category);
        category.Name = name;

        _categoryRepository.Update(category);

        await _categoryRepository.SaveChangesAsync(cancellationToken);

        ContactCategoryResponse response = _mapper.Map<ContactCategoryResponse>(category);

        return ServiceResult<ContactCategoryResponse>.Ok(
            response,
            "Category berhasil diubah.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        ContactCategory? category =
            await _categoryRepository.Query()
                .FirstOrDefaultAsync(
                    category => category.Id == id,
                    cancellationToken);

        if (category is null)
        {
            return ServiceResult<object>.Fail(
                "Category tidak ditemukan.",
                ServiceResultStatus.NotFound);
        }

        bool hasContacts = await _contactRepository.AnyAsync(
            contact => contact.ContactCategoryId == id,
            cancellationToken);

        if (hasContacts)
        {
            return ServiceResult<object>.Fail(
                "Category tidak bisa dihapus karena masih digunakan contact.",
                ServiceResultStatus.Conflict);
        }

        _categoryRepository.Remove(category);

        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return ServiceResult<object>.Ok(
            null!,
            "Category berhasil dihapus.");
    }
}
