using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CaseManager.BackgroundTasks
{
    public class ThumbnailCreator
    {
        //QueueTrigger instead?

        [FunctionName(nameof(ThumbnailCreator))]
        public async Task Run(
            [BlobTrigger(Constants.IMAGES_CONTAINER + "/{fileName}", Connection = Constants.BLOB_CONNECTION_NAME)] Stream input,
            string fileName,
            [Blob(Constants.THUMBNAILS_CONTAINER, FileAccess.Write)] BlobContainerClient thumbnailClient, ILogger log)
        {
            var extension = Path.GetExtension(fileName);
            var encoder = GetEncoder(extension);

            if (encoder != null)
            {
                var thumbnailWidth = Convert.ToInt32(Environment.GetEnvironmentVariable("THUMBNAIL_WIDTH") ?? "64");

                using (var output = new MemoryStream())
                using (var image = Image.Load(input))
                {
                    var divisor = image.Width / thumbnailWidth;
                    var height = Convert.ToInt32(Math.Round((decimal)(image.Height / divisor)));
                    image.Mutate(x => x.Resize(thumbnailWidth, height));
                    image.Save(output, encoder);
                    output.Position = 0;
                    await thumbnailClient.CreateIfNotExistsAsync();
                    await thumbnailClient.UploadBlobAsync(fileName, output);
                }
            }
            else
            {
                log.LogInformation($"No encoder support for: {fileName}");
            }

        }

        private static IImageEncoder GetEncoder(string extension)
        {
            IImageEncoder encoder = null;

            extension = extension.Replace(".", "");

            var isSupported = Regex.IsMatch(extension, "gif|png|jpe?g", RegexOptions.IgnoreCase);

            if (isSupported)
            {
                switch (extension.ToLower())
                {
                    case "png":
                        encoder = new PngEncoder();
                        break;
                    case "jpg":
                        encoder = new JpegEncoder();
                        break;
                    case "jpeg":
                        encoder = new JpegEncoder();
                        break;
                    case "gif":
                        encoder = new GifEncoder();
                        break;
                    default:
                        break;
                }
            }

            return encoder;
        }
    }
}
