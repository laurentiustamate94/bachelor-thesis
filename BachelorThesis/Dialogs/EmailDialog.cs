using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Threading.Tasks;
using BachelorThesis.Abstractions;
using BachelorThesis.Abstractions.Models;
using BachelorThesis.Helpers;
using BachelorThesis.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BachelorThesis.Dialogs
{
    [Serializable]
    public sealed class EmailDialog : IDialog<EmailDialogResult>
    {
        private readonly ICollection<string> possibleYes =
            ConfigurationManager.AppSettings["EmailDialog.PossibleYes"].Split(',');

        private readonly ICollection<string> possibleNo =
            ConfigurationManager.AppSettings["EmailDialog.PossibleNo"].Split(',');

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var loggingService = Container.Resolve<ILoggingService>();
            var activity = await result as Activity;

            if (string.IsNullOrEmpty(activity?.Text))
            {
                context.Wait(MessageReceivedAsync);

                return;
            }

            loggingService.Log(activity.Id, activity.Text);

            var translatorService = Container.Resolve<ITranslatorService>();
            var activityText = await translatorService.TranslateToEnglish(activity.Text);

            loggingService.Log(loggingService.GetLogIdByMessageId(activity.Id), LogStep.TextTranslate, activityText);

            if (possibleYes.Contains(activityText.ToLower()))
            {
                await PositiveResponse(context, translatorService);

                return;
            }

            if (possibleNo.Contains(activityText.ToLower()))
            {
                await NegativeResponse(context, translatorService);

                return;
            }

            if (new EmailAddressAttribute().IsValid(activity.Text))
            {
                context.Done(new EmailDialogResult()
                {
                    EmailAddress = activity.Text,
                    WasEmailAddressSubmitted = true
                });

                return;
            }

            var analyticsService = Container.Resolve<ITextAnalyticsService>();
            var sentiment = await analyticsService.GetSentiment(activityText);

            loggingService.Log(loggingService.GetLogIdByMessageId(activity.Id), LogStep.TextAnalytics, sentiment.ToString());

            if (sentiment <= Convert.ToDouble(ConfigurationManager.AppSettings["NegativeThreshold"]))
            {
                await NegativeResponse(context, translatorService);

                return;
            }
            else if (sentiment >= Convert.ToDouble(ConfigurationManager.AppSettings["PositiveThreshold"]))
            {
                await PositiveResponse(context, translatorService);

                return;
            }

            var wrongEmail = await translatorService
                .TranslateToLocale(ConfigurationManager.AppSettings["EmailDialog.WrongEmail"]);

            await context.PostAsync(wrongEmail);

            context.Wait(MessageReceivedAsync);
        }

        private static async Task NegativeResponse(IDialogContext context, ITranslatorService translatorService)
        {
            var noAnswer = await translatorService
                .TranslateToLocale(ConfigurationManager.AppSettings["EmailDialog.NoAnswer"]);

            await context.PostAsync(noAnswer);

            context.Done(new EmailDialogResult()
            {
                WasEmailAddressSubmitted = false
            });
        }

        private async Task PositiveResponse(IDialogContext context, ITranslatorService translatorService)
        {
            var yesAnswer = await translatorService
                .TranslateToLocale(ConfigurationManager.AppSettings["EmailDialog.YesAnswer"]);

            await context.PostAsync(yesAnswer);

            context.Wait(MessageReceivedAsync);
        }
    }
}