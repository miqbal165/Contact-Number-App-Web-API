using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.Auth;

namespace ContactNumberWebAPI.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
}