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

    public AuthService(
        IRepository<User> userRepository,
        JwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ServiceResult<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        string normalizedEmail = request.Email.Trim().ToLowerInvariant();

        bool emailExists = await _userRepository.AnyAsync(
            user => user.Email == normalizedEmail,
            cancellationToken);

        if (emailExists)
        {
            return ServiceResult<AuthResponse>.Fail(
                "Email sudah terdaftar.",
                ServiceResultStatus.Conflict);
        }

        User user = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        await _userRepository.AddAsync(user, cancellationToken);

        await _userRepository.SaveChangesAsync(cancellationToken);

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

    public async Task<ServiceResult<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        string normalizedEmail = request.Email.Trim().ToLowerInvariant();

        User? user = await _userRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Email == normalizedEmail,
                cancellationToken);

        bool passwordValid = user is not null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordValid || user is null)
        {
            return ServiceResult<AuthResponse>.Fail(
                "Email atau password salah.",
                ServiceResultStatus.Unauthorized);
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