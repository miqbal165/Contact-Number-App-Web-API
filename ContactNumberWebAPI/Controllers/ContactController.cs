using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.Contacts;
using ContactNumberWebAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactNumberWebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly IValidator<ContactCreateRequest> _createValidator;
    private readonly IValidator<ContactUpdateRequest> _updateValidator;

    public ContactsController(
        IContactService contactService,
        IValidator<ContactCreateRequest> createValidator,
        IValidator<ContactUpdateRequest> updateValidator)
    {
        _contactService = contactService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ContactResponse>>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        ServiceResult<PagedResult<ContactResponse>> result =
            await _contactService.GetAllAsync(
                search,
                categoryId,
                page,
                pageSize,
                cancellationToken);

        return StatusCode(
            (int)result.Status,
            ApiResponse<PagedResult<ContactResponse>>
                .FromServiceResult(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContactResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        ServiceResult<ContactResponse> result =
            await _contactService.GetByIdAsync(
                id,
                cancellationToken);

        return StatusCode(
            (int)result.Status,
            ApiResponse<ContactResponse>
                .FromServiceResult(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactResponse>>> Create(
        ContactCreateRequest request,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult =
            await _createValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            IReadOnlyList<string> errors =
                ValidationErrorMapper.ToMessages(validationResult);

            return BadRequest(
                ApiResponse<object>.ValidationFailure(errors));
        }

        ServiceResult<ContactResponse> result =
            await _contactService.CreateAsync(
                request,
                cancellationToken);

        if (!result.Success || result.Data is null)
        {
            return StatusCode(
                (int)result.Status,
                ApiResponse<ContactResponse>
                    .FromServiceResult(result));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data.Id },
            ApiResponse<ContactResponse>
                .FromServiceResult(result));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContactResponse>>> Update(
        Guid id,
        ContactUpdateRequest request,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult =
            await _updateValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            IReadOnlyList<string> errors =
                ValidationErrorMapper.ToMessages(validationResult);

            return BadRequest(
                ApiResponse<object>.ValidationFailure(errors));
        }

        ServiceResult<ContactResponse> result =
            await _contactService.UpdateAsync(
                id,
                request,
                cancellationToken);

        return StatusCode(
            (int)result.Status,
            ApiResponse<ContactResponse>
                .FromServiceResult(result));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        ServiceResult<object> result =
            await _contactService.DeleteAsync(
                id,
                cancellationToken);

        return StatusCode(
            (int)result.Status,
            ApiResponse<object>.FromServiceResult(result));
    }
}
