using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CurrencyEnum = Voyagr.Domain.Enums.Currency;

namespace Voyagr.Application.DTOS.Currency;

public class FavoriteCurrencyResponse
{
    public Guid Id { get; set; }

    public CurrencyEnum FromCurrency { get; set; }

    public CurrencyEnum ToCurrency { get; set; }

    public DateTime CreatedAt { get; set; }
}