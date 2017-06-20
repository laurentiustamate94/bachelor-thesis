using System.Collections.Generic;
using System.Threading.Tasks;

namespace BachelorThesis.Abstractions
{
    public interface ITextAnalyticsService
    {
        Task<IEnumerable<string>> GetKeyPhrases(string text);

        Task<double> GetSentiment(string text);
    }
}
