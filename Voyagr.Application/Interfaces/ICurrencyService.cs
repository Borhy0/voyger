using Voyagr.Application.DTOS.Currency;
using CurrencyEnum = Voyagr.Domain.Enums.Currency;

namespace Voyagr.Application.Interfaces;

public interface ICurrencyService
{
    Task<CurrencyConvertResponse> ConvertAsync(
        CurrencyEnum from,
        CurrencyEnum to,
        decimal amount);

    Task<CurrencyTrendResponse> GetTrendAsync(
        CurrencyEnum from,
        CurrencyEnum to);
}