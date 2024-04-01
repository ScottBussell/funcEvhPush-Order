using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;

namespace funcEvhPush_Order
{
    public class HealthCheck
    {
        [FunctionName("hc")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            // get event hub connection
            string evhConnStr = Environment.GetEnvironmentVariable("EVENT_HUB_CONNECTIONSTRING");

            try
            {
                // in leu of actually "pinging" the event hub, allow it to monitor/report on its own -- just make sure we have a conn str
                if (string.IsNullOrEmpty(evhConnStr))
                {
                    log.LogWarning("Missing event hub connection string");
                    LoggingRepository.SLogWarning("HealthCheck", "Event hub connection string is blank!");
                    return new BadRequestObjectResult("Event hub connection string is blank!");
                }

                // made it this far -- status is good, return OK
                log.LogInformation("HealthCheck status: OK");
                return new OkObjectResult("OK");
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "Error during health check");
                LoggingRepository.SLogWarning("HealthCheck", ex.Message);
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
