using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using BachelorThesis.Database;
using BachelorThesis.Database.Models;
using BachelorThesis.Services;

namespace BachelorThesis.DatabaseUpdateTool
{
    public sealed class UpdateWorker
    {
        private readonly Timer _timer = null;

        public UpdateWorker()
        {
            this._timer = new Timer(1000 * 60 * 10)
            {
                AutoReset = true
            };

            this._timer.Elapsed += this.ElapsedEventHandler;
        }

        public void Start()
        {
            ElapsedEventHandler(null, null);
            this._timer.Start();
        }

        public void Stop()
        {
            this._timer.Stop();
        }

        private async void ElapsedEventHandler(object sender, ElapsedEventArgs e)
        {
            var qnaMakerService = new QnAMakerService();
            var analyticsService = new TextAnalyticsService();

            var remoteKnowledgeBase = await qnaMakerService.DownloadKnowledgeBase();

            using (var dbContext = new DbContext())
            {
                foreach (var item in remoteKnowledgeBase)
                {
                    var checksum = this.GetMd5Checksum(item.Question + "|" + item.Answer);
                    var entry = dbContext.KnowledgeBase
                        .Where(x => x.PairChecksum == checksum)
                        .FirstOrDefault();

                    if (entry == null)
                    {
                        dbContext.KnowledgeBase.Add(new KnowledgeBase
                        {
                            Question = item.Question,
                            Answer = item.Answer,
                            PairChecksum = checksum,
                            Intent = item.Source,
                            Hits = 0,
                            Analysis = string.Join(",", await analyticsService.GetKeyPhrases(item.Question))
                        });

                        continue;
                    }

                    entry.Intent = item.Source;

                    dbContext.SaveChanges();
                }
            }

            await qnaMakerService.PublishKnowledgeBase();
            await qnaMakerService.TrainKnowledgeBase(null);
        }

        private string GetMd5Checksum(string data)
        {
            using (MD5 md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(data));

                return BitConverter.ToString(hash)
                    .Replace("-", String.Empty);
            }
        }
    }
}
