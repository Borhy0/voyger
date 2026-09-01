using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Voyagr.Domain.Enums;

namespace Voyagr.Domain.Entities;

public class FavoriteCurrencyPair
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Currency FromCurrency { get; set; }

    public Currency ToCurrency { get; set; }

    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
