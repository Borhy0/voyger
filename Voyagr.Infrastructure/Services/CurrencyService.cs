using System.Text.Json;
using Voyagr.Application.DTOS.Currency;
using Voyagr.Application.Interfaces;
using CurrencyEnum = Voyagr.Domain.Enums.Currency;
using Microsoft.Extensions.Caching.Memory;


namespace Voyagr.Infrastructure.Services;

public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public CurrencyService(
    HttpClient httpClient,
    IMemoryCache cache)
    {
        _httpClient = httpClient;

        _cache = cache;

        _httpClient.BaseAddress =
            new Uri("https://api.frankfurter.dev/");
    }

    public async Task<CurrencyConvertResponse> ConvertAsync(
    CurrencyEnum from,
    CurrencyEnum to,
    decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException(
                "Amount must be greater than zero."
            );
        }

        var fromCode = from.ToString();
        var toCode = to.ToString();

        if (from == to)
        {
            return new CurrencyConvertResponse
            {
                From = fromCode,
                To = toCode,
                Amount = amount,
                Rate = 1,
                ConvertedAmount = amount
            };
        }

        var cacheKey = $"fx-rate:{fromCode}:{toCode}";

        if (!_cache.TryGetValue(
                cacheKey,
                out decimal rate))
        {
            var response = await _httpClient.GetAsync(
                $"v2/rate/{fromCode}/{toCode}"
            );

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve exchange rate."
                );
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var rateResponse =
                JsonSerializer.Deserialize<
                    FrankfurterRateResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

            if (rateResponse is null)
            {
                throw new InvalidOperationException(
                    "Invalid response from currency provider."
                );
            }

            rate = rateResponse.Rate;

            _cache.Set(
                cacheKey,
                rate,
                TimeSpan.FromHours(1)
            );
        }

        var convertedAmount =
            amount * rate;

        return new CurrencyConvertResponse
        {
            From = fromCode,
            To = toCode,
            Amount = amount,
            Rate = rate,
            ConvertedAmount = convertedAmount
        };
    }
    public async Task<CurrencyTrendResponse> GetTrendAsync(
    CurrencyEnum from,
    CurrencyEnum to)
    {
        var fromCode = from.ToString();
        var toCode = to.ToString();

        if (from == to)
        {
            var today = DateTime.UtcNow.Date;

            return new CurrencyTrendResponse
            {
                From = fromCode,
                To = toCode,
                Rates = Enumerable.Range(0, 7)
                    .Select(i => new CurrencyRatePoint
                    {
                        Date = today.AddDays(-6 + i),
                        Rate = 1
                    })
                    .ToList()
            };
        }

        var toDate = DateTime.UtcNow.Date;
        var fromDate = toDate.AddDays(-6);

        var url =
            $"v2/rates?from={fromDate:yyyy-MM-dd}" +
            $"&to={toDate:yyyy-MM-dd}" +
            $"&base={fromCode}" +
            $"&quotes={toCode}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                "Unable to retrieve currency trend."
            );
        }

        var json =
            await response.Content.ReadAsStringAsync();

        var rateResponse =
            JsonSerializer.Deserialize<
                List<FrankfurterHistoricalRateResponse>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

        if (rateResponse is null)
        {
            throw new InvalidOperationException(
                "Invalid response from currency provider."
            );
        }

        return new CurrencyTrendResponse
        {
            From = fromCode,
            To = toCode,

            Rates = rateResponse
                .Select(x => new CurrencyRatePoint
                {
                    Date = x.Date,
                    Rate = x.Rate
                })
                .OrderBy(x => x.Date)
                .ToList()
        };
    }

    private class FrankfurterRateResponse
    {
        public string Date { get; set; } = string.Empty;

        public string Base { get; set; } = string.Empty;

        public string Quote { get; set; } = string.Empty;

        public decimal Rate { get; set; }
    }
    private class FrankfurterHistoricalRateResponse
    {
        public DateTime Date { get; set; }

        public string Base { get; set; } = string.Empty;

        public string Quote { get; set; } = string.Empty;

        public decimal Rate { get; set; }
    }
}