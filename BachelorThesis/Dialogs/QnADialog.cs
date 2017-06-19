using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BachelorThesis.Abstractions;
using BachelorThesis.Database;
using BachelorThesis.Database.Models;
using BachelorThesis.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using QnAMakerDialog;

namespace BachelorThesis.Dialogs
{
    [Serializable]
    public sealed class QnADialog : QnAMakerDialog<object>
    {
        private readonly ICollection<string> possibleFeedback =
            ConfigurationManager.AppSettings["QnADialog.PossibleFeedback"].Split(',');

        private readonly ICollection<string> possibleYes =
            ConfigurationManager.AppSettings["EmailDialog.PossibleYes"].Split(',');

        private bool WasLuisCalled = false;

        private string PreviousMessageId = string.Empty;

        public QnADialog()
        {
            base.SubscriptionKey = ConfigurationManager.AppSettings["QnaMakerApiSubscriptionKey"];
            base.KnowledgeBaseId = ConfigurationManager.AppSettings["QnaMakerKnowledgebaseId"];
        }

        public override Task StartAsync(IDialogContext context)
        {
            var translatorService = Container.Resolve<ITranslatorService>();
            var initialGreetingTask = Task.Run(
                () => translatorService.TranslateToLocale(ConfigurationManager.AppSettings["QnADialog.InitialGreeting"]));

            initialGreetingTask.Wait();

            context.PostAsync(initialGreetingTask.Result);

            return base.StartAsync(context);
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;
            var translatorService = Container.Resolve<ITranslatorService>();
            var loggingService = Container.Resolve<ILoggingService>();

            loggingService.Log(message.Id, message.Text);

            message.Text = await translatorService.TranslateToEnglish(message.Text);

            loggingService.Log(loggingService.GetLogIdByMessageId(message.Id), LogStep.TextTranslate, message.Text);

            await base.MessageReceived(context, item);
        }

        public override async Task DefaultMatchHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            var loggingService = Container.Resolve<ILoggingService>();
            var translatorService = Container.Resolve<ITranslatorService>();
            var message = await translatorService.TranslateToLocale(result.Answer);

            loggingService.Log(loggingService.GetLogIdByMessageId(context.Activity.Id), LogStep.QnAMaker, JsonConvert.SerializeObject(result));

            this.WasLuisCalled = false;

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [QnAMakerResponseHandler(90)]
        public async Task UncertainMatchHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            await this.MatchHandlerBase(context, originalQueryText, result);
        }

        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            await this.MatchHandlerBase(context, originalQueryText);
        }

        private async Task MatchHandlerBase(IDialogContext context, string originalQueryText, QnAMakerResult result = null)
        {
            var loggingService = Container.Resolve<ILoggingService>();

            if (result != null)
            {
                loggingService.Log(
                    loggingService.GetLogIdByMessageId(context.Activity.Id),
                    LogStep.QnAMaker,
                    JsonConvert.SerializeObject(result));
            }

            if ((this.WasLuisCalled && possibleYes.Contains(originalQueryText.ToLower())))
            {
                this.PreviousMessageId = context.Activity.Id;

                context.Call(
                    Container.Resolve<FormDialog<FeedbackDialog>>(),
                    FeedbackDialogDone);

                this.WasLuisCalled = false;

                return;
            }
            else if (possibleFeedback.Contains(originalQueryText.ToLower()))
            {
                this.PreviousMessageId = context.Activity.Id;

                context.Call(
                    Container.Resolve<FormDialog<FeedbackDialog>>(),
                    FeedbackDialogDone);

                this.WasLuisCalled = false;

                return;
            }

            this.WasLuisCalled = false;
            this.PreviousMessageId = string.Empty;

            var translatorService = Container.Resolve<ITranslatorService>();
            var forwardToLuis = await translatorService
                .TranslateToLocale(ConfigurationManager.AppSettings["QnADialog.ForwardToLuis"]);

            await context.PostAsync(forwardToLuis);

            await context.Forward(
                Container.Resolve<NaturalLanguageDialog>(),
                NaturalLanguageDialogDone,
                context.Activity.AsMessageActivity(),
                new CancellationToken());
        }

        private async Task NaturalLanguageDialogDone(IDialogContext context, IAwaitable<bool> result)
        {
            var isSuccessful = await result;

            if (!isSuccessful)
            {
                var translatorService = Container.Resolve<ITranslatorService>();
                var message = await translatorService
                    .TranslateToLocale(ConfigurationManager.AppSettings["QnADialog.DefaultMessage"]);

                await context.PostAsync(message);

                this.WasLuisCalled = true;

                return;
            }

            context.Wait(this.MessageReceived);
        }

        private async Task FeedbackDialogDone(IDialogContext context, IAwaitable<FeedbackDialog> result)
        {
            var loggingService = Container.Resolve<ILoggingService>();

            try
            {
                using (var dbContext = new DbContext())
                {
                    var response = await result;

                    var activity = context.Activity;

                    var userId = dbContext.Users
                        .AsEnumerable()
                        .Where(x => x.ConversationId == activity.Conversation.Id)
                        .Select(x => x.Id)
                        .FirstOrDefault();

                    dbContext.Feedback.Add(new Feedback
                    {
                        LoggingId = loggingService.GetLogIdByMessageId(this.PreviousMessageId),
                        UsersId = userId == 0 ? new long?() : userId,
                        RawText = response.Feedback
                    });

                    var translatorService = Container.Resolve<ITranslatorService>();
                    var message = await translatorService
                        .TranslateToLocale(ConfigurationManager.AppSettings["FeedbackDialog.FeedbackWasAdded"]);

                    await context.PostAsync(message);
                }
            }
            catch (FormCanceledException e)
            {
                loggingService.Log(e.GetType().ToString(), JsonConvert.SerializeObject(e));
            }
            catch (Exception e)
            {
                loggingService.Log(e.GetType().ToString(), JsonConvert.SerializeObject(e));
            }
            finally
            {
                context.Wait(this.MessageReceived);
            }
        }
    }
}