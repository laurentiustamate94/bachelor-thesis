using System.Collections.Generic;
using System.Threading.Tasks;

namespace BachelorThesis.Abstractions
{
    public interface IAnalysisService
    {
        Task<IEnumerable<string>> GetKeyPhrases(string text);

        Task<double> GetSentiment(string text);
    }
}
