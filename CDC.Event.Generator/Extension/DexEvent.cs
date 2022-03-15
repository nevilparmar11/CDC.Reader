using CDC.Event.Generator.Dex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CDC.Event.Generator.Extension
{
    public static class DeXEvent
    {
        public static bool PushToDeX(IConfiguration configuration, DataExchangeEvent eventData, ILogger logger)
        {
            bool isPushedToDeX = true;

            EventClient client = new EventClient(GetHttpClient().Result);
            client.BaseUrl = configuration["Event:BaseURL"];
            try
            {
                var response = client.PostAsync(eventData).Result;
                if (response.StatusCode == 200)
                {
                    logger.LogInformation("Data has been pushed to DEX successfully!");
                    isPushedToDeX = true;
                }
                else
                {
                    logger.LogInformation($"failed to push to DeX for {eventData.EventName} : {eventData.EventId}");
                    isPushedToDeX = false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                isPushedToDeX = false;
            }

            return isPushedToDeX;
        }

        private static async Task<HttpClient> GetHttpClient()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            return client;
        }
    }
}
