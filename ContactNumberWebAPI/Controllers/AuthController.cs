using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.Auth;
using ContactNumberWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContactNumberWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest registerRequest)
    {
        ServiceResult<AuthResponse> result = await _authService.RegisterAsync(registerRequest);
        return StatusCode((int)result.Status, ApiResponse<AuthResponse>.FromServiceResult(result));
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest loginRequest)
    {
        ServiceResult<AuthResponse> result = await _authService.LoginAsync(loginRequest);
        return StatusCode((int)result.Status, ApiResponse<AuthResponse>.FromServiceResult(result));
    }
}