using Voyagr.Domain.Entities;

namespace Voyagr.Application.Interfaces;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();

    Task SaveAsync(
        Guid userId,
        string token,
        DateTime expiresAt);

    Task<RefreshToken?> GetAsync(
        string token);

    Task RevokeAsync(
        RefreshToken refreshToken);
}