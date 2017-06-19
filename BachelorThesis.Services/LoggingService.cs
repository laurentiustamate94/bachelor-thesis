using System.Linq;
using BachelorThesis.Abstractions;
using BachelorThesis.Database;
using BachelorThesis.Database.Models;

namespace BachelorThesis.Services
{
    public class LoggingService : ILoggingService
    {
        public long? GetLogIdByMessageId(string messageId)
        {
            using (var dbContext = new DbContext())
            {
                var logId = dbContext.Logging
                    .AsEnumerable()
                    .Where(x => x.MessageId == messageId)
                    .Select(x => x.Id)
                    .LastOrDefault();

                return logId == 0 ? new long?() : logId;
            }
        }

        public void Log(long? logId, LogStep logStep, string message)
        {
            using (var dbContext = new DbContext())
            {
                Log(dbContext, logId, logStep, message);
            }
        }

        public void Log(string messageId, LogStep logStep, string message)
        {
            using (var dbContext = new DbContext())
            {
                Log(dbContext, this.GetLogIdByMessageId(messageId), logStep, message);
            }
        }

        public void Log(string messageId, string message)
        {
            using (var dbContext = new DbContext())
            {
                Log(dbContext, 0, LogStep.UserInput, message, messageId);
            }
        }

        private void Log(DbContext dbContext, long? logId, LogStep logStep, string message, string messageId = null)
        {
            if (!logId.HasValue)
            {
                return;
            }

            switch (logStep)
            {
                case LogStep.UserInput:
                    dbContext.Logging.Add(new Logging() { MessageId = messageId, RawText = message });
                    break;
                case LogStep.TextTranslate:
                    dbContext.Logging.Where(x => x.Id == logId).First().TranslateJson = message;
                    break;
                case LogStep.QnAMaker:
                    dbContext.Logging.Where(x => x.Id == logId).First().QnAMakerJson = message;
                    break;
                case LogStep.Luis:
                    dbContext.Logging.Where(x => x.Id == logId).First().LuisJson = message;
                    break;
                case LogStep.TextAnalysis:
                    dbContext.Logging.Where(x => x.Id == logId).First().AnalysisJson = message;
                    break;
                case LogStep.CustomAnswer:
                    dbContext.Logging.Where(x => x.Id == logId).First().CustomJson = message;
                    break;
            }

            dbContext.SaveChanges();
        }
    }
}
