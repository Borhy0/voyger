using Voyagr.Application.DTOS.Trips;
using Voyagr.Application.Interfaces;
using Voyagr.Domain.Entities;

namespace Voyagr.Application.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;
    private readonly IImageStorageService _imageStorageService;

    public TripService(
        ITripRepository tripRepository,
        IImageStorageService imageStorageService)
    {
        _tripRepository = tripRepository;
        _imageStorageService = imageStorageService;
    }

    public async Task<TripDetailDto> CreateAsync(
    Guid userId,
    CreateTripWithImagesDto request)
    {
        ValidateRequest(
            request.Destination,
            request.StartDate,
            request.EndDate,
            request.Travelers,
            request.BudgetTotal);

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

        var sortOrder = 0;

        foreach (var image in request.Images)
        {
            await using var stream = image.Content;

            var uploadResult =
                await _imageStorageService.UploadAsync(
                    stream,
                    image.FileName,
                    $"voyagr/trips/{trip.Id}");

            var tripImage = new TripImage
            {
                Id = Guid.NewGuid(),
                TripId = trip.Id,
                ImageUrl = uploadResult.Url,
                PublicId = uploadResult.PublicId,
                SortOrder = sortOrder,
                IsPrimary = sortOrder == 0,
                CreatedAt = now
            };

            trip.Images.Add(tripImage);

            sortOrder++;
        }

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
        ValidateRequest(
            request.Destination,
            request.StartDate,
            request.EndDate,
            request.Travelers,
            request.BudgetTotal);

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
            throw new ArgumentException(
                "Page must be greater than zero.");

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

    public async Task<TripOfflineResponseDto?> UpdateOfflineAsync(
        Guid userId,
        Guid tripId,
        UpdateTripOfflineRequest request)
    {
        var trip =
            await _tripRepository.GetByIdAsync(tripId);

        if (trip is null ||
            trip.UserId != userId ||
            trip.IsDeleted)
        {
            return null;
        }

        trip.IsSavedOffline = request.IsSavedOffline;
        trip.UpdatedAt = DateTime.UtcNow;

        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();

        return new TripOfflineResponseDto
        {
            TripId = trip.Id,
            IsSavedOffline = trip.IsSavedOffline
        };
    }

    private static void ValidateRequest(
        string destination,
        DateOnly startDate,
        DateOnly endDate,
        int travelers,
        decimal? budgetTotal)
    {
        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentException(
                "Destination is required.");

        if (startDate >= endDate)
            throw new ArgumentException(
                "StartDate must be before EndDate.");

        if (travelers <= 0)
            throw new ArgumentException(
                "Travelers must be greater than zero.");

        if (budgetTotal < 0)
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
            CreatedAt = trip.CreatedAt,

            Images = trip.Images
                .OrderBy(x => x.SortOrder)
                .Select(x => new TripImageDto
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    PublicId = x.PublicId,
                    SortOrder = x.SortOrder,
                    IsPrimary = x.IsPrimary
                })
                .ToList()
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

    public async Task<TripImageDto?> AddImageAsync(
    Guid userId,
    Guid tripId,
    Stream imageStream,
    string fileName)
    {
        var trip = await _tripRepository.GetByIdAsync(tripId);

        if (trip is null ||
            trip.UserId != userId ||
            trip.IsDeleted)
        {
            return null;
        }

        var sortOrder = trip.Images.Any()
            ? trip.Images.Max(x => x.SortOrder) + 1
            : 0;

        await using (imageStream)
        {
            var uploadResult =
                await _imageStorageService.UploadAsync(
                    imageStream,
                    fileName,
                    $"voyagr/trips/{trip.Id}");

            var tripImage = new TripImage
            {
                Id = Guid.NewGuid(),
                TripId = trip.Id,
                ImageUrl = uploadResult.Url,
                PublicId = uploadResult.PublicId,
                SortOrder = sortOrder,
                IsPrimary = !trip.Images.Any(),
                CreatedAt = DateTime.UtcNow
            };

            await _tripRepository.AddImageAsync(tripImage);

            await _tripRepository.SaveChangesAsync();

            return new TripImageDto
            {
                Id = tripImage.Id,
                ImageUrl = tripImage.ImageUrl,
                PublicId = tripImage.PublicId,
                SortOrder = tripImage.SortOrder,
                IsPrimary = tripImage.IsPrimary
            };
        }
    }


}