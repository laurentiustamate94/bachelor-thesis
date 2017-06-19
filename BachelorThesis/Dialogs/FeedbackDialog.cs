using System;
using System.Configuration;
using System.Threading.Tasks;
using BachelorThesis.Abstractions;
using BachelorThesis.Helpers;
using Microsoft.Bot.Builder.FormFlow;

namespace BachelorThesis.Dialogs
{
    [Serializable]
    public sealed class FeedbackDialog
    {
        public string Feedback { get; set; }

        public static IForm<FeedbackDialog> BuildForm()
        {
            var translatorService = Container.Resolve<ITranslatorService>();
            var initialGreetingTask = Task.Run(
                () => translatorService.TranslateToLocale(ConfigurationManager.AppSettings["FeedbackDialog.InitialGreeting"]));

            initialGreetingTask.Wait();

            return new FormBuilder<FeedbackDialog>()
                .Field(name: nameof(Feedback), prompt: initialGreetingTask.Result)
                .AddRemainingFields()
                .Build();
        }
    }
}