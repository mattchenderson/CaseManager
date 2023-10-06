using CaseManager.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CaseManager.BackgroundTasks
{
    public class EstimateResponse
    {
        public string CaseId { get; set; }
        public int? EstimateInHours { get; set; }
    }

    public class GetEstimate
    {
        private readonly HttpClient _quoteClient;
        public GetEstimate(IHttpClientFactory httpClientFactory)
        {
            _quoteClient = httpClientFactory.CreateClient(Constants.QUOTE_CLIENT_NAME);
        }

        [FunctionName(nameof(GetEstimate))]
        [return: Table(Constants.CASE_TABLE_NAME, Connection = Constants.TABLE_CONNECTION_NAME)]
        public async Task<CaseRecord> Run(
            [QueueTrigger(Constants.ESTIMATE_QUEUE_NAME, Connection = Constants.QUEUE_CONNECTION_NAME)] string caseId,
            [Table(Constants.CASE_TABLE_NAME, Constants.TEST_ACCOUNT, // TODO authorization check to derive account ID associated with caller. Use IBinder?
                "{QueueTrigger}", Connection = Constants.TABLE_CONNECTION_NAME)] CaseRecord caseDetails,
            ILogger log)
        {
            try
            {
                var response = await _quoteClient.GetAsync($"/?caseId={caseId}");
                if (response.IsSuccessStatusCode)
                {
                    var estimate = await response.Content.ReadFromJsonAsync<EstimateResponse>();
                    caseDetails.EstimateInHours = estimate.EstimateInHours;
                }
            }
            catch (Exception ex)
            {
                log.LogInformation("Unable to retrieve estimate from remote service.");
            }

            return caseDetails;
        }
    }
}
