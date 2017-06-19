using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using BachelorThesis.Abstractions;
using BachelorThesis.Database;
using BachelorThesis.Database.Models;
using BachelorThesis.Helpers;
using BachelorThesis.Models;
using Microsoft.Bot.Builder.Dialogs;

namespace BachelorThesis.Dialogs
{
    [Serializable]
    public sealed class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            var translatorService = Container.Resolve<ITranslatorService>();
            var lifetimeScope = Container.Resolve<ILifetimeScope>();

            var initialGreetingTask = Task.Run(
                () => translatorService.TranslateToLocale(ConfigurationManager.AppSettings["RootDialog.InitialGreeting"]));
            var emailNotificationTask = Task.Run(
                () => translatorService.TranslateToLocale(ConfigurationManager.AppSettings["RootDialog.EmailNotification"]));

            Task.WaitAll(initialGreetingTask, emailNotificationTask);

            context.PostAsync(initialGreetingTask.Result);
            context.PostAsync(emailNotificationTask.Result);

            context.Call(lifetimeScope.Resolve<EmailDialog>(), EmailDialogDone);

            return Task.CompletedTask;
        }

        private async Task EmailDialogDone(IDialogContext context, IAwaitable<EmailDialogResult> result)
        {
            var emailDialogResult = await result;
            var translatorService = Container.Resolve<ITranslatorService>();

            if (emailDialogResult.WasEmailAddressSubmitted)
            {
                var emailWasAdded = await translatorService
                    .TranslateToLocale(ConfigurationManager.AppSettings["RootDialog.EmailWasAdded"]);

                await context.PostAsync($"{emailDialogResult.EmailAddress} {emailWasAdded}");

                var activity = context.Activity;

                using (var dbContext = new DbContext())
                {
                    var existingUser = dbContext.Users
                        .Where(x => x.Email == emailDialogResult.EmailAddress)
                        .FirstOrDefault();

                    if (existingUser == null)
                    {
                        dbContext.Users.Add(new Users
                        {
                            ConversationId = activity.Conversation.Id,
                            Email = emailDialogResult.EmailAddress
                        });
                    }
                    else
                    {
                        existingUser.ConversationId = activity.Conversation.Id;
                    }

                    dbContext.SaveChanges();
                }
            }

            context.Call(Container.Resolve<QnADialog>(), QnADialogDone);
        }

        private Task QnADialogDone(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }
    }
}