using System;
using System.Collections.Generic;
using System.Text;

namespace BachelorThesis.Abstractions
{
    public enum LogStep
    {
        UserInput,
        TextTranslate,
        QnAMaker,
        Luis,
        TextAnalysis,
        CustomAnswer
    }
}
