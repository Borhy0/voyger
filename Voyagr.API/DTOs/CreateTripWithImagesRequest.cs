using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace Voyagr.API.DTOs
{
    public class CreateTripWithImagesRequest
    {
        [Required]
        public string Destination { get; set; } = string.Empty;

        public string? Country { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Travelers must be greater than zero.")]
        public int Travelers { get; set; }

        [Range(0, double.MaxValue,
            ErrorMessage = "BudgetTotal cannot be negative.")]
        public decimal? BudgetTotal { get; set; }

        public bool IsSavedOffline { get; set; } = false;

        public List<IFormFile> Images { get; set; } = new();
    }
}
