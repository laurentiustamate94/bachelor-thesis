using System.Threading.Tasks;

namespace BachelorThesis.Abstractions
{
    public interface ITranslatorService
    {
        Task<string> TranslateToEnglish(string text);

        Task<string> TranslateToLocale(string text);
    }
}
