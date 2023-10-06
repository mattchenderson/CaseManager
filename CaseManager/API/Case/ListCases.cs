using Azure.Data.Tables;
using CaseManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CaseManager.API.Case
{
    public static class ListCases
    {
        [FunctionName(nameof(ListCases))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cases")] HttpRequest req,
            [Table(Constants.CASE_TABLE_NAME, Connection = Constants.TABLE_CONNECTION_NAME)] TableClient casesTable,
            ILogger log)
        {
            // TODO authorization check to get account ID associated with caller
            var accountId = Constants.TEST_ACCOUNT;

            var caseUris = casesTable.Query<CaseRecord>($"PartitionKey eq '{accountId}'").Select(record => $"{req.Host.Value}/api/cases/{record.RowKey}");

            return new OkObjectResult(caseUris);
        }
    }
}
