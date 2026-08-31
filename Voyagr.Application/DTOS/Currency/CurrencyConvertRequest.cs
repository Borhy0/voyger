using System.ComponentModel.DataAnnotations;
using CurrencyEnum = Voyagr.Domain.Enums.Currency;

namespace Voyagr.Application.DTOS.Currency;

public class CurrencyConvertRequest
{
    public CurrencyEnum From { get; set; }

    public CurrencyEnum To { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
}