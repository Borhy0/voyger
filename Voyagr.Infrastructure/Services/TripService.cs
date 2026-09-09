using Voyagr.Application.DTOS.Trips;
using Voyagr.Application.Interfaces;
using Voyagr.Domain.Entities;

namespace Voyagr.Application.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;

    public TripService(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<TripDetailDto> CreateAsync(
        Guid userId,
        CreateTripRequest request)
    {
        ValidateRequest(request);

        var now = DateTime.UtcNow;

        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Destination = request.Destination.Trim(),
            Country = request.Country,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Travelers = request.Travelers,
            BudgetTotal = request.BudgetTotal,
            IsSavedOffline = request.IsSavedOffline,
            IsDeleted = false,
            DeletedAt = null,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _tripRepository.AddAsync(trip);
        await _tripRepository.SaveChangesAsync();

        return MapToDetailDto(trip);
    }

    public async Task<TripDetailDto?> GetByIdAsync(
        Guid userId,
        Guid tripId)
    {
        var trip = await _tripRepository.GetByIdAsync(tripId);

        if (trip is null ||
            trip.UserId != userId ||
            trip.IsDeleted)
        {
            return null;
        }

        return MapToDetailDto(trip);
    }

    public async Task<TripDetailDto?> UpdateAsync(
        Guid userId,
        Guid tripId,
        CreateTripRequest request)
    {
        ValidateRequest(request);

        var trip = await _tripRepository.GetByIdAsync(tripId);

        if (trip is null ||
            trip.UserId != userId ||
            trip.IsDeleted)
        {
            return null;
        }

        trip.Destination = request.Destination.Trim();
        trip.Country = request.Country;
        trip.Latitude = request.Latitude;
        trip.Longitude = request.Longitude;
        trip.StartDate = request.StartDate;
        trip.EndDate = request.EndDate;
        trip.Travelers = request.Travelers;
        trip.BudgetTotal = request.BudgetTotal;
        trip.IsSavedOffline = request.IsSavedOffline;
        trip.UpdatedAt = DateTime.UtcNow;

        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();

        return MapToDetailDto(trip);
    }

    public async Task<TripListResponseDto> GetPagedAsync(
        Guid userId,
        int page,
        int pageSize)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than zero.");

        if (pageSize < 1)
            throw new ArgumentException(
                "PageSize must be greater than zero.");

        var result =
            await _tripRepository.GetPagedByUserIdAsync(
                userId,
                page,
                pageSize);

        var totalPages =
            (int)Math.Ceiling(
                result.TotalCount / (double)pageSize);

        return new TripListResponseDto
        {
            Data = result.Trips
                .Select(MapToListItemDto)
                .ToList(),

            Pagination = new TripPaginationDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = result.TotalCount,
                TotalPages = totalPages
            }
        };
    }

    public async Task<List<TripSummaryDto>> GetUpcomingAsync(
        Guid userId)
    {
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var trips =
            await _tripRepository
                .GetUpcomingByUserIdAsync(userId, today);

        return trips
            .Select(MapToSummaryDto)
            .ToList();
    }

    public async Task<List<TripSummaryDto>> GetPastAsync(
        Guid userId)
    {
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var trips =
            await _tripRepository
                .GetPastByUserIdAsync(userId, today);

        return trips
            .Select(MapToSummaryDto)
            .ToList();
    }

    public async Task<List<TripSummaryDto>> GetDeletedAsync(
        Guid userId)
    {
        var trips =
            await _tripRepository
                .GetDeletedByUserIdAsync(userId);

        return trips
            .Select(MapToSummaryDto)
            .ToList();
    }

    public async Task<bool> DeleteAsync(
        Guid userId,
        Guid tripId)
    {
        var trip =
            await _tripRepository.GetByIdAsync(tripId);

        if (trip is null ||
            trip.UserId != userId)
        {
            return false;
        }

        if (trip.IsDeleted)
            return false;

        trip.IsDeleted = true;
        trip.DeletedAt = DateTime.UtcNow;
        trip.UpdatedAt = DateTime.UtcNow;

        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RecoverAsync(
        Guid userId,
        Guid tripId)
    {
        var trip =
            await _tripRepository.GetByIdAsync(tripId);

        if (trip is null ||
            trip.UserId != userId)
        {
            return false;
        }

        if (!trip.IsDeleted)
            return false;

        trip.IsDeleted = false;
        trip.DeletedAt = null;
        trip.UpdatedAt = DateTime.UtcNow;

        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();

        return true;
    }

    private static void ValidateRequest(
        CreateTripRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Destination))
            throw new ArgumentException(
                "Destination is required.");

        if (request.StartDate >= request.EndDate)
            throw new ArgumentException(
                "StartDate must be before EndDate.");

        if (request.Travelers <= 0)
            throw new ArgumentException(
                "Travelers must be greater than zero.");

        if (request.BudgetTotal < 0)
            throw new ArgumentException(
                "BudgetTotal cannot be negative.");
    }

    private static TripDetailDto MapToDetailDto(
        Trip trip)
    {
        return new TripDetailDto
        {
            Id = trip.Id,
            Destination = trip.Destination,
            Country = trip.Country,
            Latitude = trip.Latitude,
            Longitude = trip.Longitude,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Travelers = trip.Travelers,
            BudgetTotal = trip.BudgetTotal,
            IsSavedOffline = trip.IsSavedOffline,
            CreatedAt = trip.CreatedAt
        };
    }

    private static TripListItemDto MapToListItemDto(
        Trip trip)
    {
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var status =
            today < trip.StartDate
                ? "upcoming"
                : today > trip.EndDate
                    ? "past"
                    : "ongoing";

        return new TripListItemDto
        {
            Id = trip.Id,
            Destination = trip.Destination,
            Country = trip.Country,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Travelers = trip.Travelers,
            BudgetTotal = trip.BudgetTotal,
            Status = status
        };
    }

    private static TripSummaryDto MapToSummaryDto(
        Trip trip)
    {
        return new TripSummaryDto
        {
            Id = trip.Id,
            Destination = trip.Destination,
            Country = trip.Country,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Travelers = trip.Travelers
        };
    }
}