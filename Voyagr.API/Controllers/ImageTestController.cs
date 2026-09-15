using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyagr.Application.Interfaces;

namespace Voyagr.API.Controllers
{
    [ApiController]
    [Route("api/v1/images")]
    [Authorize]
    public class ImageTestController: ControllerBase
    {
        private readonly IImageStorageService _imageStorageService;

        public ImageTestController(
            IImageStorageService imageStorageService)
        {
            _imageStorageService = imageStorageService;
        }

        [HttpPost("test-upload")]
        public async Task<IActionResult> TestUpload(
            IFormFile image)
        {
            if (image is null || image.Length == 0)
            {
                return BadRequest(new
                {
                    message = "Image is required."
                });
            }

            await using var stream = image.OpenReadStream();

            var result =
                await _imageStorageService.UploadAsync(
                    stream,
                    image.FileName,
                    "voyagr/test");

            return Ok(new
            {
                data = result
            });
        }
    }
}
