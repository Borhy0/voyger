using Microsoft.AspNetCore.Http;

namespace Voyagr.API.DTOs.Trips;

public class AddTripImageRequest
{
    public IFormFile Image { get; set; } = null!;
}