using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyagr.Application.DTOS.Currency;

namespace Voyagr.Application.Interfaces
{
    public interface IFavoriteCurrencyService
    {
        Task<List<FavoriteCurrencyResponse>> GetFavoritesAsync(
        Guid userId);

        Task<FavoriteCurrencyResponse> AddFavoriteAsync(
            Guid userId,
            FavoriteCurrencyRequest request);

        Task DeleteFavoriteAsync(
            Guid userId,
            Guid favoriteId);
    }
}
