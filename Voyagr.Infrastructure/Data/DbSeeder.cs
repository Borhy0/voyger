using Microsoft.EntityFrameworkCore;
using Voyagr.Domain.Entities;
using Voyagr.Domain.Enums;

namespace Voyagr.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext context)
    {
        await context.Database.MigrateAsync();

        // -------------------------
        // Demo User
        // -------------------------

        var demoUser =
            await context.Users
                .FirstOrDefaultAsync(
                    x => x.Email == "demo@voyagr.com");

        if (demoUser is null)
        {
            demoUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Demo",
                LastName = "User",
                Email = "demo@voyagr.com",
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        "Demo123!"),
                PreferredCurrency = "USD",
                Units = "metric",
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(demoUser);

            await context.SaveChangesAsync();
        }

        // -------------------------
        // Demo Trips
        // -------------------------

        var hasTrips =
            await context.Trips
                .AnyAsync(x => x.UserId == demoUser.Id);

        if (!hasTrips)
        {
            var today =
                DateOnly.FromDateTime(
                    DateTime.UtcNow);

            var trips = new List<Trip>
            {
                new Trip
                {
                    Id = Guid.NewGuid(),
                    UserId = demoUser.Id,
                    Destination = "Tokyo",
                    Country = "Japan",
                    Latitude = 35.6762,
                    Longitude = 139.6503,
                    StartDate = today.AddDays(10),
                    EndDate = today.AddDays(17),
                    Travelers = 2,
                    BudgetTotal = 3000,
                    IsSavedOffline = false,
                    IsDeleted = false,
                    DeletedAt = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new Trip
                {
                    Id = Guid.NewGuid(),
                    UserId = demoUser.Id,
                    Destination = "Paris",
                    Country = "France",
                    Latitude = 48.8566,
                    Longitude = 2.3522,
                    StartDate = today,
                    EndDate = today.AddDays(5),
                    Travelers = 2,
                    BudgetTotal = 2500,
                    IsSavedOffline = true,
                    IsDeleted = false,
                    DeletedAt = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new Trip
                {
                    Id = Guid.NewGuid(),
                    UserId = demoUser.Id,
                    Destination = "Istanbul",
                    Country = "Turkey",
                    Latitude = 41.0082,
                    Longitude = 28.9784,
                    StartDate = today.AddDays(-20),
                    EndDate = today.AddDays(-14),
                    Travelers = 3,
                    BudgetTotal = 1500,
                    IsSavedOffline = false,
                    IsDeleted = false,
                    DeletedAt = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new Trip
                {
                    Id = Guid.NewGuid(),
                    UserId = demoUser.Id,
                    Destination = "Dubai",
                    Country = "UAE",
                    Latitude = 25.2048,
                    Longitude = 55.2708,
                    StartDate = today.AddDays(30),
                    EndDate = today.AddDays(35),
                    Travelers = 4,
                    BudgetTotal = 4000,
                    IsSavedOffline = false,
                    IsDeleted = true,
                    DeletedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Trips.AddRangeAsync(trips);

            await context.SaveChangesAsync();
        }

        // -------------------------
        // Demo Favorite Currencies
        // -------------------------

        var hasFavorites =
            await context.FavoriteCurrencyPairs
                .AnyAsync(
                    x => x.UserId == demoUser.Id);

        if (!hasFavorites)
        {
            var favoritePairs =
                new List<FavoriteCurrencyPair>
                {
                    new FavoriteCurrencyPair
                    {
                        Id = Guid.NewGuid(),
                        UserId = demoUser.Id,
                        FromCurrency = Currency.USD,
                        ToCurrency = Currency.EGP,
                        CreatedAt = DateTime.UtcNow
                    },

                    new FavoriteCurrencyPair
                    {
                        Id = Guid.NewGuid(),
                        UserId = demoUser.Id,
                        FromCurrency = Currency.EUR,
                        ToCurrency = Currency.EGP,
                        CreatedAt = DateTime.UtcNow
                    },

                    new FavoriteCurrencyPair
                    {
                        Id = Guid.NewGuid(),
                        UserId = demoUser.Id,
                        FromCurrency = Currency.USD,
                        ToCurrency = Currency.JPY,
                        CreatedAt = DateTime.UtcNow
                    }
                };

            await context.FavoriteCurrencyPairs
                .AddRangeAsync(favoritePairs);

            await context.SaveChangesAsync();
        }
    }
}