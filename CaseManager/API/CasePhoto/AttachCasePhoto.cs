using Azure.Data.Tables;
using Azure.Storage.Blobs;
using CaseManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CaseManager.API.CasePhoto
{
    public static class AttachCasePhoto
    {

        [FunctionName(nameof(AttachCasePhoto))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "cases/{caseID}/attach/{fileName}")] HttpRequest req,
            [Blob(Constants.IMAGES_CONTAINER, FileAccess.Write, Connection = Constants.BLOB_CONNECTION_NAME)] BlobContainerClient imageContainer,
            [Table(Constants.IMAGE_METADATA_TABLE_NAME, Connection = Constants.TABLE_CONNECTION_NAME)] TableClient table,
            string caseID,
            string fileName,
            ILogger log)
        {
            // TODO authorization check to get account ID associated with caller
            var accountId = Constants.TEST_ACCOUNT;

            var extension = Path.GetExtension(fileName);
            if (Regex.IsMatch(extension, "gif|png|jpe?g", RegexOptions.IgnoreCase))
            {
                var identifier = Guid.NewGuid().ToString();

                var blobName = $"{identifier}.{extension}";

                await imageContainer.UploadBlobAsync(blobName, req.Body);
                await table.AddEntityAsync(new ImageMetadata
                {
                    PartitionKey = caseID,
                    RowKey = identifier,
                    Title = fileName,
                    ContentUri = $"{req.Host.Value}/cases/{caseID}/photos/{blobName}"
                });
                return new AcceptedResult();
            }
            else
            {
                return new BadRequestResult();
            }
        }
    }
}
