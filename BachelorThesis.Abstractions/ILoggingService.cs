using System;
using System.Collections.Generic;
using System.Text;

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
