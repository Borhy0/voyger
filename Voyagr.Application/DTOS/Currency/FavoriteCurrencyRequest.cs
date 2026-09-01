using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyEnum = Voyagr.Domain.Enums.Currency;

namespace Voyagr.Application.DTOS.Currency
{
    public class FavoriteCurrencyRequest
    {
        [Required]
        public CurrencyEnum FromCurrency { get; set; }

        [Required]
        public CurrencyEnum ToCurrency { get; set; }
    }

}
