using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.ContactCategories;
using ContactNumberWebAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactNumberWebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/contact-categories")]
public class ContactCategoriesController : ControllerBase
{
    private readonly IContactCategoryService _categoryService;
    private readonly IValidator<ContactCategoryCreateRequest> _createValidator;
    private readonly IValidator<ContactCategoryUpdateRequest> _updateValidator;

    public ContactCategoriesController(
        IContactCategoryService categoryService,
        IValidator<ContactCategoryCreateRequest> createValidator,
        IValidator<ContactCategoryUpdateRequest> updateValidator)
    {
        _categoryService = categoryService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ContactCategoryResponse>>>> GetAll(
        CancellationToken cancellationToken)
    {
        ServiceResult<IReadOnlyList<ContactCategoryResponse>> result = await _categoryService
            .GetAllAsync(cancellationToken);

        return StatusCode(
            (int)result.Status,
            ApiResponse<IReadOnlyList<ContactCategoryResponse>>
                .FromServiceResult(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContactCategoryResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        ServiceResult<ContactCategoryResponse> result = await _categoryService.GetByIdAsync(id, cancellationToken);

        return StatusCode(
            (int)result.Status,
            ApiResponse<ContactCategoryResponse>
                .FromServiceResult(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactCategoryResponse>>> Create(
        ContactCategoryCreateRequest request,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            IReadOnlyList<string> errors = ValidationErrorMapper.ToMessages(validationResult);

            return BadRequest(ApiResponse<object>.ValidationFailure(errors));
        }

        ServiceResult<ContactCategoryResponse> result = await _categoryService.CreateAsync(request, cancellationToken);

        if (!result.Success || result.Data is null)
        {
            return StatusCode(
                (int)result.Status,
                ApiResponse<ContactCategoryResponse>
                    .FromServiceResult(result));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data.Id },
            ApiResponse<ContactCategoryResponse>
                .FromServiceResult(result));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContactCategoryResponse>>> Update(
        Guid id,
        ContactCategoryUpdateRequest request,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            IReadOnlyList<string> errors = ValidationErrorMapper.ToMessages(validationResult);

            return BadRequest(ApiResponse<object>.ValidationFailure(errors));
        }

        ServiceResult<ContactCategoryResponse> result = await _categoryService
            .UpdateAsync(id, request, cancellationToken);

        return StatusCode(
            (int)result.Status,
            ApiResponse<ContactCategoryResponse>
                .FromServiceResult(result));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        ServiceResult<object> result = await _categoryService.DeleteAsync(id, cancellationToken);

        return StatusCode((int)result.Status, ApiResponse<object>.FromServiceResult(result));
    }
}
