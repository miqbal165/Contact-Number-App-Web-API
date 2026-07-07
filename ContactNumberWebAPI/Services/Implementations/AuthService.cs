using ContactNumberWebAPI.Common;
using ContactNumberWebAPI.DTOs.Auth;
using ContactNumberWebAPI.Helpers;
using ContactNumberWebAPI.Models;
using ContactNumberWebAPI.Repositories.Interfaces;
using ContactNumberWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactNumberWebAPI.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IRepository<User> userRepository, JwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    
    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        string email = request.Email.Trim().ToLower();
        bool emailExists = await _userRepository.AnyAsync(user => user.Email == email);

        if (emailExists)
        {
            return ServiceResult<AuthResponse>
                .Fail(
                    "Email sudah terdaftar.",
                    ServiceResultStatus.Conflict
                );
        }

        User user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return ServiceResult<AuthResponse>.Created(
                new AuthResponse
                {
                    AccessToken = _jwtTokenGenerator.GenerateAccessToken(user),
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role
                },
                "Register berhasil."
            );
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        string email = request.Email.Trim().ToLower();
        User? user = await _userRepository.Query().FirstOrDefaultAsync(user => user.Email == email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ServiceResult<AuthResponse>.Fail(
                "Email atau password salah",
                ServiceResultStatus.Unauthorized
            );
        }

        return ServiceResult<AuthResponse>.Ok(
            new AuthResponse
            {
                AccessToken = _jwtTokenGenerator.GenerateAccessToken(user),
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            },
            "Login berhasil."
        );
    }
}