using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.Auth;
using ContactNumberWebAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;


namespace ContactNumberWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult =
            await _registerValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            IReadOnlyList<string> errors =
                ValidationErrorMapper.ToMessages(validationResult);

            return BadRequest(
                ApiResponse<object>.ValidationFailure(errors));
        }

        ServiceResult<AuthResponse> result =
            await _authService.RegisterAsync(
                request,
                cancellationToken);

        return StatusCode(
            (int)result.Status,
            ApiResponse<AuthResponse>.FromServiceResult(result));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult =
            await _loginValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            IReadOnlyList<string> errors =
                ValidationErrorMapper.ToMessages(validationResult);

            return BadRequest(
                ApiResponse<object>.ValidationFailure(errors));
        }

        ServiceResult<AuthResponse> result =
            await _authService.LoginAsync(
                request,
                cancellationToken);

        return StatusCode(
            (int)result.Status,
            ApiResponse<AuthResponse>.FromServiceResult(result));
    }
}