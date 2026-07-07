using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.Contacts;
using ContactNumberWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactNumberWebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ContactResponse>>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        ServiceResult<PagedResult<ContactResponse>> result = await _contactService.GetAllAsync(
            search,
            categoryId,
            page,
            pageSize);

        return StatusCode((int)result.Status, ApiResponse<PagedResult<ContactResponse>>.FromServiceResult(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContactResponse>>> GetById(Guid id)
    {
        ServiceResult<ContactResponse> result = await _contactService.GetByIdAsync(id);
        return StatusCode((int)result.Status, ApiResponse<ContactResponse>.FromServiceResult(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactResponse>>> Create(ContactCreateRequest request)
    {
        ServiceResult<ContactResponse> result = await _contactService.CreateAsync(request);

        if (!result.Success || result.Data is null)
        {
            return StatusCode((int)result.Status, ApiResponse<ContactResponse>.FromServiceResult(result));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data.Id },
            ApiResponse<ContactResponse>.FromServiceResult(result));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContactResponse>>> Update(Guid id, ContactUpdateRequest request)
    {
        ServiceResult<ContactResponse> result = await _contactService.UpdateAsync(id, request);
        return StatusCode((int)result.Status, ApiResponse<ContactResponse>.FromServiceResult(result));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        ServiceResult<object> result = await _contactService.DeleteAsync(id);
        return StatusCode((int)result.Status, ApiResponse<object>.FromServiceResult(result));
    }
}