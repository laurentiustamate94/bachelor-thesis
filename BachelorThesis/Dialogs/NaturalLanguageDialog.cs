using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using BachelorThesis.Abstractions;
using BachelorThesis.Database;
using BachelorThesis.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;

namespace BachelorThesis.Dialogs
{
    [Serializable]
    public sealed class NaturalLanguageDialog : LuisDialog<bool>
    {
        public NaturalLanguageDialog(ILuisService service)
            : base(service)
        {
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public Task None(IDialogContext context, LuisResult result)
        {
            context.Done(false);

            return Task.CompletedTask;
        }

        private async Task IntentBase(IDialogContext context, LuisResult result)
        {
            var loggingService = Container.Resolve<ILoggingService>();

            loggingService.Log(
                loggingService.GetLogIdByMessageId(context.Activity.Id),
                LogStep.Luis,
                JsonConvert.SerializeObject(result));

            if (result.TopScoringIntent.Score <= Convert.ToDouble(ConfigurationManager.AppSettings["PositiveThreshold"]))
            {
                await this.AnswerByAnalysis(context, result.Entities);

                return;
            }

            await this.AnswerByIntent(context, result.TopScoringIntent, result.Entities);
        }

        private async Task AnswerByIntent(IDialogContext context, IntentRecommendation topScoringIntent, IList<EntityRecommendation> entities)
        {
            using (var dbContext = new DbContext())
            {
                var loggingService = Container.Resolve<ILoggingService>();
                var translatorService = Container.Resolve<ITranslatorService>();

                var knowledgeBase = dbContext.KnowledgeBase
                    .AsEnumerable()
                    .Where(x => x.Intent == topScoringIntent.Intent)
                    .Where(x =>
                    {
                        var questionTokens = x.Question.Split(' ');

                        foreach (var token in questionTokens)
                        {
                            if (entities.Where(y => y.Entity == token).Any())
                            {
                                return true;
                            }
                        }

                        return false;
                    })
                    .OrderByDescending(x => x.Hits);

                if (!knowledgeBase.Any())
                {
                    context.Done(false);

                    return;
                }

                var entry = knowledgeBase.First();
                var answer = await translatorService.TranslateToLocale(entry.Answer);

                loggingService.Log(
                    loggingService.GetLogIdByMessageId(context.Activity.Id),
                    LogStep.CustomAnswer,
                    JsonConvert.SerializeObject(entry));

                await context.PostAsync(answer);

                entry.Hits++;

                dbContext.SaveChanges();

                context.Done(true);
            }
        }

        private async Task AnswerByAnalysis(IDialogContext context, IList<EntityRecommendation> entities)
        {
            using (var dbContext = new DbContext())
            {
                var loggingService = Container.Resolve<ILoggingService>();
                var translatorService = Container.Resolve<ITranslatorService>();
                var analysisService = Container.Resolve<IAnalysisService>();
                var keyPhrases = await analysisService.GetKeyPhrases(context.Activity.AsMessageActivity().Text);

                loggingService.Log(
                    loggingService.GetLogIdByMessageId(context.Activity.Id),
                    LogStep.TextAnalysis,
                    JsonConvert.SerializeObject(keyPhrases));

                var knowledgeBase = dbContext.KnowledgeBase
                    .AsEnumerable()
                    .Where(x =>
                    {
                        var analysisTokens = x.Analysis.Split(',');

                        foreach (var token in analysisTokens)
                        {
                            if (keyPhrases.Where(y => y == token).Any())
                            {
                                return true;
                            }
                        }

                        return false;
                    })
                    .Where(x =>
                    {
                        var questionTokens = x.Question.Split(' ');

                        foreach (var token in questionTokens)
                        {
                            if (entities.Where(y => y.Entity == token).Any())
                            {
                                return true;
                            }
                        }

                        return false;
                    })
                    .OrderByDescending(x => x.Hits);

                if (!knowledgeBase.Any())
                {
                    context.Done(false);

                    return;
                }

                var entry = knowledgeBase.First();
                var answer = await translatorService.TranslateToLocale(entry.Answer);

                loggingService.Log(
                    loggingService.GetLogIdByMessageId(context.Activity.Id),
                    LogStep.CustomAnswer,
                    JsonConvert.SerializeObject(entry));

                await context.PostAsync(answer);

                entry.Hits++;

                dbContext.SaveChanges();

                context.Done(true);
            }
        }

        [LuisIntent("EnvironmentIntent")]
        public async Task EnvironmentIntent(IDialogContext context, LuisResult result)
        {
            await this.IntentBase(context, result);
        }

        [LuisIntent("AssignmentObjectivesIntent")]
        public async Task AssignmentObjectivesIntent(IDialogContext context, LuisResult result)
        {
            await this.IntentBase(context, result);
        }

        [LuisIntent("AssignmentRequirementIntent")]
        public async Task AssignmentRequirementIntent(IDialogContext context, LuisResult result)
        {
            await this.IntentBase(context, result);
        }

        [LuisIntent("AssignmentSkeletonIntent")]
        public async Task AssignmentSkeletonIntent(IDialogContext context, LuisResult result)
        {
            await this.IntentBase(context, result);
        }

        [LuisIntent("AssignmentExampleIntent")]
        public async Task AssignmentExampleIntent(IDialogContext context, LuisResult result)
        {
            await this.IntentBase(context, result);
        }

        [LuisIntent("AssignmentCheckerIntent")]
        public async Task AssignmentCheckerIntent(IDialogContext context, LuisResult result)
        {
            await this.IntentBase(context, result);
        }

        [LuisIntent("AssignmentScoringIntent")]
        public async Task AssignmentScoringIntent(IDialogContext context, LuisResult result)
        {
            await this.IntentBase(context, result);
        }

        [LuisIntent("OtherIntent")]
        public async Task OtherIntent(IDialogContext context, LuisResult result)
        {
            await this.IntentBase(context, result);
        }
    }
}