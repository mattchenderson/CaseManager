using Azure.Data.Tables;
using CaseManager.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace CaseManager.API.CasePhoto
{
    public class PhotoMetadataRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public static class UpdatePhotoMetadata
    {
        [FunctionName(nameof(UpdatePhotoMetadata))]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "cases/{caseID}/photos/{imageName}")] PhotoMetadataRequest req,
            string caseID,
            string imageName,
            [Table(Constants.IMAGE_METADATA_TABLE_NAME, Connection = Constants.TABLE_CONNECTION_NAME)] TableClient imageMetadataTable,
            ILogger log)
        {
            var imageId = Path.GetFileNameWithoutExtension(imageName);

            var oldRecord = (await imageMetadataTable.GetEntityAsync<ImageMetadata>(caseID, imageId)).Value;

            oldRecord.Title = req.Title;
            oldRecord.Description = req.Description;

            await imageMetadataTable.UpdateEntityAsync(oldRecord, Azure.ETag.All);
        }
    }
}
