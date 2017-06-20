using BachelorThesis.Abstractions.Models;

namespace BachelorThesis.Abstractions
{
    public interface ILoggingService
    {
        long? GetLogIdByMessageId(string messageId);

        void Log(long? logId, LogStep logStep, string message);

        void Log(string messageId, LogStep logStep, string message);

        void Log(string messageId, string message);
    }
}
