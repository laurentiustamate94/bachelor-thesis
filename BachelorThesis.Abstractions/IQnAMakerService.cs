using System.Collections.Generic;
using System.Threading.Tasks;
using BachelorThesis.Abstractions.Models;

namespace BachelorThesis.Abstractions
{
    public interface IQnAMakerService
    {
        Task<IEnumerable<KnowledgebaseModel>> DownloadKnowledgeBase();

        Task PublishKnowledgeBase();

        Task TrainKnowledgeBase(IEnumerable<TrainingModel> feedbackRecords);
    }
}
