using Azure.Data.Tables;
using Azure.Storage.Blobs;
using CaseManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CaseManager
{
#if DEBUG

    /// <summary>
    /// This is for local testing purposes only. It's meant to populate the storage account with some basic information to exercise the APIs concerned with reads.
    /// </summary>
    public class AddTestData
    {
        private readonly HttpClient _debugClient;

        public AddTestData(IHttpClientFactory httpClientFactory)
        {
            this._debugClient = httpClientFactory.CreateClient("debug");
        }

        [FunctionName(nameof(AddTestData))]
        public async Task Run([HttpTrigger (AuthorizationLevel.Admin, "get","post", Route = "addTestData")] HttpRequest req,
            [Table(Constants.CASE_TABLE_NAME, Connection = Constants.TABLE_CONNECTION_NAME)] TableClient casesTable,
            [Blob(Constants.IMAGES_CONTAINER, FileAccess.Write, Connection = Constants.BLOB_CONNECTION_NAME)] BlobContainerClient imageContainer,
            [Table(Constants.IMAGE_METADATA_TABLE_NAME, Connection = Constants.TABLE_CONNECTION_NAME)] TableClient imageMetadataTable,
            ILogger log)
        {
            log.LogInformation("Adding test data for debugging purposes");
            var accountId = Constants.TEST_ACCOUNT;
            var caseId = Guid.NewGuid().ToString();
            var imageId = Guid.NewGuid().ToString();
            var blobName = $"{imageId}.png";

            var caseEntry = new CaseRecord
            {
                PartitionKey = accountId,
                RowKey = caseId,
                CustomerDescription = "This is a test case to make it easier to debug the application",
            };

            await casesTable.AddEntityAsync<CaseRecord>(caseEntry);

            var samplePhoto = await _debugClient.GetStreamAsync("https://raw.githubusercontent.com/Azure/azure-functions-cli/master/src/Azure.Functions.Cli/npm/assets/azure-functions-logo-color-raster.png");

            await imageContainer.CreateIfNotExistsAsync();
            await imageContainer.UploadBlobAsync(blobName, samplePhoto);
            await imageMetadataTable.AddEntityAsync<ImageMetadata>(new ImageMetadata
            {
                PartitionKey = caseId,
                RowKey = imageId,
                Title = "Old Azure Functions logo",
                Description = "Just for nostalgic fun, this test data uses the original Azure Functions logo.",
                ContentUri = $"{req.Host.Value}/api/cases/{caseId}/photos/{blobName}"
            });
        }
    }
#endif
}
