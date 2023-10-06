using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace CaseManager.API.CasePhoto
{
    public static class GetCasePhoto
    {
        [FunctionName(nameof(GetCasePhoto))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cases/{caseID}/photos/{imageName}")] HttpRequest req,
            string imageName,
            IBinder binder,
            ILogger log)
        {
            // TODO authorization check to only allow callers matching the case account
            var accountId = Constants.TEST_ACCOUNT;

            string container;
            if (req.Query["size"] == "thumbnail")
            {
                container = Constants.THUMBNAILS_CONTAINER;
            }
            else
            {
                container = Constants.IMAGES_CONTAINER;
            }

            var attribute = new BlobAttribute(container + "/{imageName}", FileAccess.Read);
            attribute.Connection = Constants.BLOB_CONNECTION_NAME;

            var blobData = await binder.BindAsync<byte[]>(attribute);

            if (blobData != null)
            {
                return new FileContentResult(blobData, "application/octet-stream")
                {
                    FileDownloadName = imageName
                };
            }
            else
            {
                return new NotFoundResult();
            }

        }
    }
}