using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Application.DTOS.Trips
{
    public class UpdateTripOfflineRequest
    {
        [Required]
        public bool IsSavedOffline { get; set; }
    }
}
