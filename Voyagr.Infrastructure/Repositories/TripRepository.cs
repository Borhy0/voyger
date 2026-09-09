using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voyagr.Application.Interfaces;
using Voyagr.Domain.Entities;
using Voyagr.Infrastructure.Data;

namespace Voyagr.Infrastructure.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly AppDbContext _context;

        public TripRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Trip?> GetByIdAsync(Guid id)
        {
            return await _context.Trips
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Trip>> GetByUserIdAsync(
            Guid userId)
        {
            return await _context.Trips
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.StartDate)
                .ToListAsync();
        }

        public async Task AddAsync(Trip trip)
        {
            await _context.Trips.AddAsync(trip);
        }

        public void Update(Trip trip)
        {
            _context.Trips.Update(trip);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(List<Trip> Trips, int TotalCount)>
        GetPagedByUserIdAsync(
            Guid userId,
            int page,
            int pageSize)
            {
                var query = _context.Trips
                    .Where(x =>
                        x.UserId == userId &&
                        !x.IsDeleted);

                var totalCount = await query.CountAsync();

                var trips = await query
                    .OrderBy(x => x.StartDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (trips, totalCount);
        }

        public async Task<List<Trip>> GetUpcomingByUserIdAsync(
    Guid userId,
    DateOnly today)
        {
            return await _context.Trips
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsDeleted &&
                    x.StartDate >= today)
                .OrderBy(x => x.StartDate)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetPastByUserIdAsync(
            Guid userId,
            DateOnly today)
        {
            return await _context.Trips
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsDeleted &&
                    x.EndDate < today)
                .OrderByDescending(x => x.EndDate)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetDeletedByUserIdAsync(
            Guid userId)
        {
            return await _context.Trips
                .Where(x =>
                    x.UserId == userId &&
                    x.IsDeleted)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
        }
    }
}
