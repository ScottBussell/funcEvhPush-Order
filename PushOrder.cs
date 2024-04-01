using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web.Http;
using System.Text;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;

namespace funcEvhPush_Order
{
    public static class PushOrder
    {
        #region Private Properties for range validation

        #endregion

        [FunctionName("PushOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("PushOrder HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            LoggingRepository.SLogInfo("ConsumeEvent: Body", requestBody);

            dynamic data;
            try
            {
                data = JsonConvert.DeserializeObject(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to deserialize JSON object from body.");
                LoggingRepository.SLogError("DeserializeObject", string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, requestBody));
                return new BadRequestErrorMessageResult("Error parsing JSON payload: " + ex.Message);
            }

            string responseMessage = null;
            OrderModel mOrder = null;

            if (ValidateOrder(ref requestBody, ref responseMessage, ref mOrder))
            {
                string evhConnectionString = Environment.GetEnvironmentVariable("EVENT_HUB_CONNECTIONSTRING");
                string hubName = Environment.GetEnvironmentVariable("HUB_NAME");

                EventHubProducerClient producerClient = new EventHubProducerClient(evhConnectionString, hubName);

                // Create a batch of events 
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(requestBody))))
                {
                    // if it is too large for the batch
                    responseMessage = string.Format("OrderID: {0} is too large for the batch and cannot be sent.", mOrder.sourceOrderId);
                    log.LogError(responseMessage, mOrder.GetLoggingInfo);
                    LoggingRepository.SLogError("EventHubProducerClient", responseMessage, mOrder.GetLoggingInfo);
                    return new BadRequestErrorMessageResult(responseMessage);
                }
                else
                {
                    try
                    {
                        // Use the producer client to send the batch of events to the event hub
                        await producerClient.SendAsync(eventBatch);
                        log.LogInformation("Order pushed to event hub", mOrder.GetLoggingInfo);
                        LoggingRepository.SLogInfo("EventHubProducerClient", "Order pushed to event hub", mOrder.GetLoggingInfo);
                    }
                    finally
                    {
                        await producerClient.DisposeAsync();
                    }
                }

                responseMessage = "Order pushed successfully";
                return new OkObjectResult(responseMessage);
            }
            else
            {
                // responseMessage should already contain formatted errors encountered
                log.LogError(responseMessage, requestBody);
                LoggingRepository.SLogError("ValidateTransaction", responseMessage, mOrder.GetLoggingInfo);
                return new BadRequestErrorMessageResult(responseMessage);
            }
        }

        #region Private Methods

        /// <summary>
        /// Validate the passed json object
        /// </summary>
        /// <param name="requestBody">Full request body containing valid json object</param>
        /// <param name="resultMsg">List of any failed validations (output parameter)</param>
        /// <param name="oModel">Order model (output parameter)</param>
        /// <returns></returns>
        private static bool ValidateOrder(ref string requestBody, ref string resultMsg, ref OrderModel oModel)
        {             
            // catch for nothing passed
            if (string.IsNullOrEmpty(requestBody))
            {
                resultMsg = "Missing or invalid JSON order object.  ";
                return false;
            }

            // catch for invalid JSON object/properties
            try
            {
                oModel = JsonConvert.DeserializeObject<OrderModel>(requestBody);
            }
            catch (Exception ex)
            {
                resultMsg = "Error parsing JSON order object.  " + ex.Message;
                return false;
            }

            // handle SNS passed messages
            if (!string.IsNullOrEmpty(oModel.messageId))
            {
                string snsMsgId = oModel.messageId;
                string msg = oModel.message;

                oModel = JsonConvert.DeserializeObject<OrderModel>(msg);
                // add msgId back in (for debug logging, etc)
                oModel.messageId = snsMsgId;
            }

            // check ranges for fields
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(oModel.source))
            {
                sb.Append("Missing or invalid source.  ");
            }

            if (string.IsNullOrEmpty(oModel.sourceOrderId))
            {
                sb.Append("Missing or invalid sourceOrderId.  ");
            }

            resultMsg = sb.ToString();
            return sb.Length == 0;
        }

        #endregion
    }
}
