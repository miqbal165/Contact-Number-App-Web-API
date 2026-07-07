using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.ContactCategories;
using ContactNumberWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactNumberWebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/contact-categories")]
public class ContactCategoryController : ControllerBase
{
    private readonly IContactCategoryService _contactCategoryService;

    public ContactCategoryController(IContactCategoryService contactCategoryService)
    {
        _contactCategoryService = contactCategoryService;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ContactCategoryResponse>>>> GetAll()
    {
        ServiceResult<IReadOnlyList<ContactCategoryResponse>> result = await _contactCategoryService.GetAllAsync();
        return StatusCode((int)result.Status, ApiResponse<IReadOnlyList<ContactCategoryResponse>>.FromServiceResult(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContactCategoryResponse>>> GetById(Guid id)
    {
        ServiceResult<ContactCategoryResponse> result = await _contactCategoryService.GetByIdAsync(id);
        return StatusCode((int)result.Status, ApiResponse<ContactCategoryResponse>.FromServiceResult(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactCategoryResponse>>> Create(ContactCategoryCreateRequest request)
    {
        ServiceResult<ContactCategoryResponse> result = await _contactCategoryService.CreateAsync(request);

        if (!result.Success || result.Data is null)
        {
            return StatusCode((int)result.Status, ApiResponse<ContactCategoryResponse>.FromServiceResult(result));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data.Id },
            ApiResponse<ContactCategoryResponse>.FromServiceResult(result));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContactCategoryResponse>>> Update(
        Guid id,
        ContactCategoryUpdateRequest request)
    {
        ServiceResult<ContactCategoryResponse> result = await _contactCategoryService.UpdateAsync(id, request);
        return StatusCode((int)result.Status, ApiResponse<ContactCategoryResponse>.FromServiceResult(result));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        ServiceResult<object> result = await _contactCategoryService.DeleteAsync(id);
        return StatusCode((int)result.Status, ApiResponse<object>.FromServiceResult(result));
    }
}