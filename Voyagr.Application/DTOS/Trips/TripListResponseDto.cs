using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Application.DTOS.Trips
{
    public class TripListResponseDto
    {
        public List<TripListItemDto> Data { get; set; } = new();

        public TripPaginationDto Pagination { get; set; } = new();
    }
}
