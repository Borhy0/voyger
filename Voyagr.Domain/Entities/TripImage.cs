using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Domain.Entities
{
    public class TripImage
    {
        public Guid Id { get; set; }

        public Guid TripId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string? PublicId { get; set; }

        public int SortOrder { get; set; }

        public bool IsPrimary { get; set; }

        public DateTime CreatedAt { get; set; }

        public Trip Trip { get; set; } = null!;
}
}
