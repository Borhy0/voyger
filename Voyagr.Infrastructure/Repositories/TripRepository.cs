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
    }
}
