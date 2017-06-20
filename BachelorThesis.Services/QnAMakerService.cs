using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BachelorThesis.Abstractions;
using BachelorThesis.Abstractions.Models;
using Newtonsoft.Json;

namespace BachelorThesis.Services
{
    public sealed class QnAMakerService : IQnAMakerService
    {
        private readonly string baseUri = null;
        private readonly string subscriptionKey = null;

        public QnAMakerService()
        {
            this.subscriptionKey = ConfigurationManager.AppSettings["QnaMakerApiSubscriptionKey"];
            this.baseUri = string.Format(
                "{0}/{1}",
                "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases",
                ConfigurationManager.AppSettings["QnaMakerKnowledgebaseId"]);
        }

        public async Task<IEnumerable<KnowledgebaseModel>> DownloadKnowledgeBase()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key",
                    this.subscriptionKey);

                var response = await client.GetAsync(this.baseUri);
                var result = await response.Content.ReadAsStringAsync();

                response = await client.GetAsync(result.Replace("\"", ""));
                result = await response.Content.ReadAsStringAsync();

                return result.Split('\r', '\n')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Skip(1)
                    .Select(x =>
                    {
                        var tokens = x.Split('\t');

                        return new KnowledgebaseModel
                        {
                            Question = tokens[0],
                            Answer = tokens[1],
                            Source = tokens[2],
                        };
                    });
            }
        }

        public async Task PublishKnowledgeBase()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key",
                    this.subscriptionKey);

                await client.PutAsync(this.baseUri, null);
            }
        }

        public async Task TrainKnowledgeBase(IEnumerable<TrainingModel> feedbackRecords)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key",
                    this.subscriptionKey);

                var data = JsonConvert.SerializeObject(new
                {
                    feedbackRecords = feedbackRecords
                });

                var request = new HttpRequestMessage
                {
                    Method = new HttpMethod("PATCH"),
                    RequestUri = new Uri(this.baseUri + "/train"),
                    Content = new StringContent(data, Encoding.UTF8, "application/json"),
                };

                var response = await client.SendAsync(request);
            }
        }
    }
}
