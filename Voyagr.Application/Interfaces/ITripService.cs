using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyagr.Application.DTOS.Trips;

namespace Voyagr.Application.Interfaces
{
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
    }
}
