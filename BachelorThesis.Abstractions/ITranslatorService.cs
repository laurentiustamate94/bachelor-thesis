using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BachelorThesis.Abstractions
{
    public interface ITranslatorService
    {
        Task<string> TranslateToEnglish(string text);

        Task<string> TranslateToLocale(string text);
    }
}
