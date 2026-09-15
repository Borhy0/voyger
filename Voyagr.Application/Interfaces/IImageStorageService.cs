using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<ImageUploadResponse> UploadAsync(
            Stream imageStream,
            string fileName,
            string folder);

        Task DeleteAsync(string publicId);
    }

    public class ImageUploadResponse
    {
        public string Url { get; set; } = string.Empty;

        public string PublicId { get; set; } = string.Empty;
    }
}
