using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Application.DTOS.Trips
{
    public class TripImageDto
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string? PublicId { get; set; }

        public int SortOrder { get; set; }

        public bool IsPrimary { get; set; }
    }
}
