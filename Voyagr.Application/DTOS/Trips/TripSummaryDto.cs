using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Application.DTOS.Trips
{
    public class TripSummaryDto
    {
        public Guid Id { get; set; }

        public string Destination { get; set; } = string.Empty;

        public string? Country { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int Travelers { get; set; }
    }
}
