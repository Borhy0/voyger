using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Voyagr.Application.Interfaces;
using Voyagr.Domain.Entities;
using Voyagr.Infrastructure.Data;

namespace Voyagr.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _context;

    public RefreshTokenService(AppDbContext context)
    {
        _context = context;
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

    public async Task SaveAsync(
        Guid userId,
        string token,
        DateTime expiresAt)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        await _context.RefreshTokens.AddAsync(refreshToken);

        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task RevokeAsync(
        RefreshToken refreshToken)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;

        _context.RefreshTokens.Update(refreshToken);

        await _context.SaveChangesAsync();
    }
}