using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Application.DTOS.Currency
{
    public class CurrencyConvertResponse
    {
        public string From { get; set; } = string.Empty;

        public string To { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public decimal Rate { get; set; }

        public decimal ConvertedAmount { get; set; }
    }
}
