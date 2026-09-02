using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Domain.Entities
{
    public class Trip
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Destination { get; set; } = string.Empty;
        public string? Country { get; set; }
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int Travelers { get; set; }

        public decimal? BudgetTotal { get; set; }

        public bool IsSavedOffline { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;

    }
}
