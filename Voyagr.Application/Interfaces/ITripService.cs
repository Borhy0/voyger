using Voyagr.Application.DTOS.Trips;

namespace Voyagr.Application.Interfaces;

public interface ITripService
{
    Task<TripDetailDto> CreateAsync(
        Guid userId,
        CreateTripRequest request);

    Task<TripDetailDto?> GetByIdAsync(
        Guid userId,
        Guid tripId);

    Task<TripDetailDto?> UpdateAsync(
        Guid userId,
        Guid tripId,
        CreateTripRequest request);

    Task<TripListResponseDto> GetPagedAsync(
        Guid userId,
        int page,
        int pageSize);

    Task<List<TripSummaryDto>> GetUpcomingAsync(
        Guid userId);

    Task<List<TripSummaryDto>> GetPastAsync(
        Guid userId);

    Task<List<TripSummaryDto>> GetDeletedAsync(
        Guid userId);

    Task<bool> DeleteAsync(
        Guid userId,
        Guid tripId);

    Task<bool> RecoverAsync(
        Guid userId,
        Guid tripId);
}