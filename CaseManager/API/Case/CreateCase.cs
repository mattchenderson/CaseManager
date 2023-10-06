using CaseManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CaseManager.API.Case
{
    public class CaseCreationRequest
    {
        public string Description { get; set; }
    }

    public static class CreateCase
    {
        [FunctionName(nameof(CreateCase))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "cases/new")] CaseCreationRequest request,
            [Table(Constants.CASE_TABLE_NAME, Connection = Constants.TABLE_CONNECTION_NAME)] IAsyncCollector<CaseRecord> caseCollector,
            [Queue(Constants.ESTIMATE_QUEUE_NAME, Connection = Constants.QUEUE_CONNECTION_NAME)] IAsyncCollector<string> queueMessageCollector,
            ILogger log)
        {
            // TODO authorization check to get account ID associated with caller
            var accountId = Constants.TEST_ACCOUNT;

            var caseId = Guid.NewGuid().ToString();

            var caseEntry = new CaseRecord
            {
                PartitionKey = accountId,
                RowKey = caseId,
                CustomerDescription = request.Description,
            };

            await caseCollector.AddAsync(caseEntry);
            await queueMessageCollector.AddAsync(caseId);

            return new CreatedResult($"cases/{caseId}", request);
        }
    }
}
