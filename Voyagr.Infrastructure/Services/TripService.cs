using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyagr.Application.DTOS.Trips;
using Voyagr.Application.Interfaces;
using Voyagr.Domain.Entities;

namespace Voyagr.Infrastructure.Services
{
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
            ValidateTrip(request);

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

                CreatedAt = DateTime.UtcNow
            };

            await _tripRepository.AddAsync(trip);

            await _tripRepository.SaveChangesAsync();

            return MapToDetailDto(trip);
        }

        public async Task<TripDetailDto?> GetByIdAsync(
            Guid userId,
            Guid tripId)
        {
            var trip =
                await _tripRepository.GetByIdAsync(tripId);

            if (trip is null || trip.UserId != userId)
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
            ValidateTrip(request);

            var trip =
                await _tripRepository.GetByIdAsync(tripId);

            if (trip is null || trip.UserId != userId)
            {
                return null;
            }

            trip.Destination =
                request.Destination.Trim();

            trip.Country =
                request.Country;

            trip.Latitude =
                request.Latitude;

            trip.Longitude =
                request.Longitude;

            trip.StartDate =
                request.StartDate;

            trip.EndDate =
                request.EndDate;

            trip.Travelers =
                request.Travelers;

            trip.BudgetTotal =
                request.BudgetTotal;

            trip.IsSavedOffline =
                request.IsSavedOffline;

            _tripRepository.Update(trip);

            await _tripRepository.SaveChangesAsync();

            return MapToDetailDto(trip);
        }

        private static void ValidateTrip(
            CreateTripRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Destination))
            {
                throw new ArgumentException(
                    "Destination is required."
                );
            }

            if (request.StartDate >= request.EndDate)
            {
                throw new ArgumentException(
                    "StartDate must be before EndDate."
                );
            }

            if (request.Travelers <= 0)
            {
                throw new ArgumentException(
                    "Travelers must be greater than zero."
                );
            }

            if (request.BudgetTotal < 0)
            {
                throw new ArgumentException(
                    "BudgetTotal cannot be negative."
                );
            }
        }

        private static TripDetailDto MapToDetailDto(
            Trip trip)
        {
            return new TripDetailDto
            {
                Id = trip.Id,

                Destination =
                    trip.Destination,

                Country =
                    trip.Country,

                Latitude =
                    trip.Latitude,

                Longitude =
                    trip.Longitude,

                StartDate =
                    trip.StartDate,

                EndDate =
                    trip.EndDate,

                Travelers =
                    trip.Travelers,

                BudgetTotal =
                    trip.BudgetTotal,

                IsSavedOffline =
                    trip.IsSavedOffline,

                CreatedAt =
                    trip.CreatedAt
            };
        }
    }
}
