using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BachelorThesis.Abstractions;
using Newtonsoft.Json;

namespace BachelorThesis.Services
{
    public sealed class AnalysisService : IAnalysisService
    {
        public ILoggingService LoggingService { get; set; }

        private readonly string baseUri = null;

        public AnalysisService()
        {
            this.baseUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/";
        }

        public async Task<IEnumerable<string>> GetKeyPhrases(string text)
        {
            try
            {
                return await ActualGetKeyPhrases(text);
            }
            catch (Exception e)
            {
                this.LoggingService.Log(e.GetType().ToString(), JsonConvert.SerializeObject(e));

                return null;
            }
        }

        public async Task<double> GetSentiment(string text)
        {
            try
            {
                return await ActualGetSentiment(text);
            }
            catch (Exception e)
            {
                this.LoggingService.Log(e.GetType().ToString(), JsonConvert.SerializeObject(e));

                return -1;
            }
        }

        private async Task<IEnumerable<string>> ActualGetKeyPhrases(string text)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key",
                    ConfigurationManager.AppSettings["TextAnalysisApiKey"]);

                var request = new
                {
                    documents = new List<object>()
                    {
                        new
                        {
                            language = "en",
                            id = "1",
                            text = text
                        }
                    }
                };

                var response = await client.PostAsync(
                    this.baseUri + "keyPhrases",
                    new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json"));
                var responseContent = await response.Content.ReadAsStringAsync();

                var jsonData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return (jsonData.documents as IEnumerable<dynamic>).First().keyPhrases as IEnumerable<string>;
            }
        }

        private async Task<double> ActualGetSentiment(string text)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key",
                    ConfigurationManager.AppSettings["TextAnalysisApiKey"]);

                var request = new
                {
                    documents = new List<object>()
                    {
                        new
                        {
                            language = "en",
                            id = "1",
                            text = text
                        }
                    }
                };

                var response = await client.PostAsync(
                    this.baseUri + "sentiment",
                    new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json"));
                var responseContent = await response.Content.ReadAsStringAsync();

                var jsonData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return Convert.ToDouble((jsonData.documents as IEnumerable<dynamic>).First().score);
            }
        }
    }
}
