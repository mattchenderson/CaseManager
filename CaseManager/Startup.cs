using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(CaseManager.Startup))]

namespace CaseManager
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var quoteServiceBase = builder.GetContext().Configuration["QUOTE_SERVICE_BASE"];
            builder.Services.AddHttpClient(Constants.QUOTE_CLIENT_NAME, client =>
            {
                client.BaseAddress = new Uri(quoteServiceBase);
            });
#if DEBUG
            builder.Services.AddHttpClient("debug");
#endif
        }
    }
}