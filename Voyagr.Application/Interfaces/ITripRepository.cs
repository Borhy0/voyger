using Voyagr.Domain.Entities;

namespace Voyagr.Application.Interfaces;

public interface ITripRepository
{
    Task<Trip?> GetByIdAsync(Guid id);

    Task<List<Trip>> GetByUserIdAsync(Guid userId);

    Task<(List<Trip> Trips, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId,
        int page,
        int pageSize);

    Task<List<Trip>> GetUpcomingByUserIdAsync(
        Guid userId,
        DateOnly today);

    Task<List<Trip>> GetPastByUserIdAsync(
        Guid userId,
        DateOnly today);

    Task<List<Trip>> GetDeletedByUserIdAsync(
        Guid userId);

    Task AddAsync(Trip trip);

    Task AddImageAsync(TripImage image);
    void Update(Trip trip);

    Task SaveChangesAsync();
}