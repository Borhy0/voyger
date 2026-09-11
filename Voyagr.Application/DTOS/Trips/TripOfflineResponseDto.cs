using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Application.DTOS.Trips
{
    public class TripOfflineResponseDto
    {
        public Guid TripId { get; set; }
        public bool IsSavedOffline { get; set; }
    }
}
