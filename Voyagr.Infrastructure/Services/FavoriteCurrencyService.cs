using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyagr.Application.DTOS.Currency;
using Voyagr.Application.Interfaces;
using Voyagr.Domain.Entities;

namespace Voyagr.Infrastructure.Services
{
    public class FavoriteCurrencyService : IFavoriteCurrencyService
    {
        private readonly IFavoriteCurrencyRepository
       _favoriteRepository;

        public FavoriteCurrencyService(
            IFavoriteCurrencyRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<List<FavoriteCurrencyResponse>>
            GetFavoritesAsync(Guid userId)
        {
            var favorites =
                await _favoriteRepository
                    .GetByUserIdAsync(userId);

            return favorites
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<FavoriteCurrencyResponse>
            AddFavoriteAsync(
                Guid userId,
                FavoriteCurrencyRequest request)
        {
            if (request.FromCurrency == request.ToCurrency)
            {
                throw new InvalidOperationException(
                    "From and To currencies cannot be the same."
                );
            }

            var exists =
                await _favoriteRepository
                    .GetByUserAndPairAsync(
                        userId,
                        request.FromCurrency,
                        request.ToCurrency);

            if (exists is not null)
            {
                throw new InvalidOperationException(
                    "This currency pair is already in favorites."
                );
            }

            var favorite = new FavoriteCurrencyPair
            {
                Id = Guid.NewGuid(),

                UserId = userId,

                FromCurrency =
                    request.FromCurrency,

                ToCurrency =
                    request.ToCurrency,

                CreatedAt = DateTime.UtcNow
            };

            await _favoriteRepository
                .AddAsync(favorite);

            await _favoriteRepository
                .SaveChangesAsync();

            return MapToResponse(favorite);
        }

        public async Task DeleteFavoriteAsync(
            Guid userId,
            Guid favoriteId)
        {
            var favorite =
                await _favoriteRepository
                    .GetByIdAsync(favoriteId);

            if (favorite is null ||
                favorite.UserId != userId)
            {
                throw new KeyNotFoundException(
                    "Favorite currency pair not found."
                );
            }

            _favoriteRepository.Delete(favorite);

            await _favoriteRepository
                .SaveChangesAsync();
        }

        private static FavoriteCurrencyResponse
            MapToResponse(
                FavoriteCurrencyPair favorite)
        {
            return new FavoriteCurrencyResponse
            {
                Id = favorite.Id,

                FromCurrency =
                    favorite.FromCurrency,

                ToCurrency =
                    favorite.ToCurrency,

                CreatedAt =
                    favorite.CreatedAt
            };
        }
    }
}
