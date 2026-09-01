using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyagr.Domain.Entities;
using Voyagr.Domain.Enums;


namespace Voyagr.Application.Interfaces
{
    public interface IFavoriteCurrencyRepository
    {
        Task<List<FavoriteCurrencyPair>> GetByUserIdAsync(
        Guid userId);

        Task<FavoriteCurrencyPair?> GetByIdAsync(
            Guid id);

        Task<FavoriteCurrencyPair?> GetByUserAndPairAsync(
            Guid userId,
            Currency fromCurrency,
            Currency toCurrency);

        Task AddAsync(
            FavoriteCurrencyPair favorite);

        void Delete(
            FavoriteCurrencyPair favorite);

        Task SaveChangesAsync();
    }
}
