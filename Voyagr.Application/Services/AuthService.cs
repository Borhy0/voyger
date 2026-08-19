//using BCrypt.Net;
//using Voyagr.Application.DTOS.Auth;
using Voyagr.Application.DTOS.Auth;
using Voyagr.Application.Interfaces;
using Voyagr.Domain.Entities;

namespace Voyagr.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> SignupAsync(
        SignupRequest request)
    {
        var email = request.Email
            .Trim()
            .ToLower();

        var exists =
            await _userRepository
                .ExistsByEmailAsync(email);

        if (exists)
        {
            throw new InvalidOperationException(
                "Email already exists."
            );
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(
            request.Password
        );

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = passwordHash,
            PreferredCurrency = "USD",
            Units = "metric",
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        await _userRepository.SaveChangesAsync();

        var token =
            _jwtService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            User = MapUser(user)
        };
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request)
    {
        var email = request.Email
            .Trim()
            .ToLower();

        var user =
            await _userRepository
                .GetByEmailAsync(email);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password."
            );
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash
        );

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password."
            );
        }

        var token =
            _jwtService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            User = MapUser(user)
        };
    }

    public async Task<UserResponse>
        GetCurrentUserAsync(Guid userId)
    {
        var user =
            await _userRepository
                .GetByIdAsync(userId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found."
            );
        }

        return MapUser(user);
    }

    private static UserResponse MapUser(
        User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PreferredCurrency =
                user.PreferredCurrency,
            Units = user.Units
        };
    }
}