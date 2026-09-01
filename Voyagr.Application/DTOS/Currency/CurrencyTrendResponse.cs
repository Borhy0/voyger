using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Application.DTOS.Currency
{
    public class CurrencyTrendResponse
    {
        public string From { get; set; } = string.Empty;

        public string To { get; set; } = string.Empty;

        public List<CurrencyRatePoint> Rates { get; set; } = new();
    }

    public class CurrencyRatePoint
    {
        public DateTime Date { get; set; }

        public decimal Rate { get; set; }
    }
}
