using System.Text.Json;
using Voyagr.Application.DTOS.Currency;
using Voyagr.Application.Interfaces;
using CurrencyEnum = Voyagr.Domain.Enums.Currency;

namespace Voyagr.Infrastructure.Services;

public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;

    public CurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient;

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
            JsonSerializer.Deserialize<FrankfurterRateResponse>(
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

        var convertedAmount =
            amount * rateResponse.Rate;

        return new CurrencyConvertResponse
        {
            From = fromCode,
            To = toCode,
            Amount = amount,
            Rate = rateResponse.Rate,
            ConvertedAmount = convertedAmount
        };
    }

    private class FrankfurterRateResponse
    {
        public string Date { get; set; } = string.Empty;

        public string Base { get; set; } = string.Empty;

        public string Quote { get; set; } = string.Empty;

        public decimal Rate { get; set; }
    }
}