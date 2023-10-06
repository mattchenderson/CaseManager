using Azure.Data.Tables;
using CaseManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CaseManager.API.Case
{
    public class CaseDetailsResponse
    {
        public string AccountId { get; set; }
        public string CaseId { get; set; }
        public string Description { get; set; }
        public int? Estimate { get; set; }
        public IEnumerable<AttachmentDetails> AttachedPhotos { get; set; }
    }

    public class AttachmentDetails
    {
        public Uri Content { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public static class GetCase
    {
        [FunctionName(nameof(GetCase))]
        public static CaseDetailsResponse Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cases/{caseId}")] HttpRequest req,
            string caseId,
            [Table(Constants.CASE_TABLE_NAME, Constants.TEST_ACCOUNT, // TODO authorization check to derive account ID associated with caller
                "{caseId}", Connection = Constants.TABLE_CONNECTION_NAME)] CaseRecord caseDetails,
            [Table(Constants.IMAGE_METADATA_TABLE_NAME, Connection = Constants.TABLE_CONNECTION_NAME)] TableClient imageMetadataTable,
            ILogger log)
        {
            // TODO authorization check to get account ID associated with caller
            var accountId = Constants.TEST_ACCOUNT;

            var attachments = new List<AttachmentDetails>();

            var images = imageMetadataTable.Query<ImageMetadata>($"PartitionKey eq '{caseId}'");

            foreach (var image in images)
            {
                attachments.Add(new AttachmentDetails
                {
                    Title = image.Title,
                    Description = image.Description,
                    Content = new Uri(image.ContentUri)
                });
            }

            return new CaseDetailsResponse
            {
                AccountId = caseDetails.PartitionKey,
                CaseId = caseDetails.RowKey,
                Description = caseDetails.CustomerDescription,
                Estimate = caseDetails.EstimateInHours,
                AttachedPhotos = attachments
            };
        }
    }
}