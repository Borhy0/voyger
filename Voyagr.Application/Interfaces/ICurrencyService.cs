using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyEnum = Voyagr.Domain.Enums.Currency;
using Voyagr.Application.DTOS.Currency;

namespace Voyagr.Application.Interfaces
{
    public interface ICurrencyService
    {
        Task<CurrencyConvertResponse> ConvertAsync(
            CurrencyEnum from,
            CurrencyEnum to,
            decimal amount);
    }
}
