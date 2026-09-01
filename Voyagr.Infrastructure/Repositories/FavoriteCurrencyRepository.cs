using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voyagr.Application.Interfaces;
using Voyagr.Domain.Entities;
using Voyagr.Domain.Enums;
using Voyagr.Infrastructure.Data;

namespace Voyagr.Infrastructure.Repositories
{
    public class FavoriteCurrencyRepository : IFavoriteCurrencyRepository
    {
        private readonly AppDbContext _context;

        public FavoriteCurrencyRepository(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FavoriteCurrencyPair>>
            GetByUserIdAsync(Guid userId)
        {
            return await _context.FavoriteCurrencyPairs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<FavoriteCurrencyPair?>
            GetByIdAsync(Guid id)
        {
            return await _context.FavoriteCurrencyPairs
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<FavoriteCurrencyPair?>
            GetByUserAndPairAsync(
                Guid userId,
                Currency fromCurrency,
                Currency toCurrency)
        {
            return await _context.FavoriteCurrencyPairs
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.FromCurrency == fromCurrency &&
                    x.ToCurrency == toCurrency);
        }

        public async Task AddAsync(
            FavoriteCurrencyPair favorite)
        {
            await _context.FavoriteCurrencyPairs
                .AddAsync(favorite);
        }

        public void Delete(
            FavoriteCurrencyPair favorite)
        {
            _context.FavoriteCurrencyPairs
                .Remove(favorite);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
