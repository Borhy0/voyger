using Voyagr.Application.DTOS.Auth;
using Voyagr.Application.Interfaces;
using Voyagr.Domain.Entities;

namespace Voyagr.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
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

        var passwordHash =
            BCrypt.Net.BCrypt.HashPassword(
                request.Password
            );

        var user = new User
        {
            Id = Guid.NewGuid(),

            FirstName = request.FirstName.Trim(),

            LastName = request.LastName.Trim(),

            Email = email,

            PasswordHash = passwordHash,

            PreferredCurrency =
                request.PreferredCurrency.ToUpper(),

            Units = request.Units,

            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        await _userRepository.SaveChangesAsync();

        // Generate Access Token
        var accessToken =
            _jwtService.GenerateToken(user);

        // Generate Refresh Token
        var refreshToken =
            _refreshTokenService.GenerateRefreshToken();

        // Save Refresh Token
        await _refreshTokenService.SaveAsync(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(30)
        );

        return new AuthResponse
        {
            AccessToken = accessToken,

            RefreshToken = refreshToken,

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

        var passwordValid =
            BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash
            );

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password."
            );
        }

        // Generate Access Token
        var accessToken =
            _jwtService.GenerateToken(user);

        // Generate Refresh Token
        var refreshToken =
            _refreshTokenService.GenerateRefreshToken();

        // Save Refresh Token
        await _refreshTokenService.SaveAsync(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(30)
        );

        return new AuthResponse
        {
            AccessToken = accessToken,

            RefreshToken = refreshToken,

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

            FirstName = user.FirstName,

            LastName = user.LastName,

            Email = user.Email,

            PreferredCurrency =
                user.PreferredCurrency,

            Units = user.Units
        };
    }
    public async Task LogoutAsync(string refreshToken)
    {
        var token = await _refreshTokenService.GetAsync(refreshToken);

        if (token is null)
        {
            return;
        }

        if (token.RevokedAt != null)
        {
            return;
        }

        token.RevokedAt = DateTime.UtcNow;

        await _refreshTokenService.RevokeAsync(token);
    }
}