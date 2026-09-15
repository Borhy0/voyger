using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Voyagr.Application.Interfaces;
using Voyagr.Infrastructure.Data.Configurations;

namespace Voyagr.Infrastructure.Services
{
    public class CloudinaryImageStorageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageStorageService(
            IOptions<CloudinarySettings> settings)
        {
            var account = new Account(
                settings.Value.CloudName,
                settings.Value.ApiKey,
                settings.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResponse> UploadAsync(
            Stream imageStream,
            string fileName,
            string folder)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(
                    fileName,
                    imageStream),

                Folder = folder
            };

            var result =
                await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new Exception(
                    $"Cloudinary upload failed: {result.Error.Message}");
            }

            return new ImageUploadResponse
            {
                Url = result.SecureUrl?.ToString()
                      ?? result.Url?.ToString()
                      ?? string.Empty,

                PublicId = result.PublicId
            };
        }

        public async Task DeleteAsync(string publicId)
        {
            var deleteParams =
                new DeletionParams(publicId);

            var result =
                await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                throw new Exception(
                    $"Cloudinary delete failed: {result.Error.Message}");
            }
        }
    }
}
