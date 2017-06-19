using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using BachelorThesis.Abstractions;
using BachelorThesis.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace BachelorThesis
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public ILifetimeScope LifetimeScope { get; set; }

        public ILoggingService LoggingService { get; set; }

        /// <summary>
        /// The main entry point of the chatbot
        /// </summary>
        /// <param name="activity">Activity from the bot context</param>
        /// <returns>A http status code based on execution status</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                await this.HandleActivity(activity);

                return base.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                await this.SendMessageToConversation(activity);

                this.LoggingService.Log(e.GetType().ToString(), JsonConvert.SerializeObject(e));

                return base.Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private async Task SendMessageToConversation(Activity activity)
        {
            var translatorService = this.LifetimeScope.Resolve<ITranslatorService>();
            var serviceUrl = new Uri(activity.ServiceUrl);
            var appCredentials = new MicrosoftAppCredentials(
                ConfigurationManager.AppSettings["MicrosoftAppId"],
                ConfigurationManager.AppSettings["MicrosoftAppPassword"]);
            var connectorClient = new ConnectorClient(serviceUrl, appCredentials);

            var reply = activity.CreateReply();
            reply.Text = await translatorService
                .TranslateToLocale(ConfigurationManager.AppSettings["GeneralErrorMessage"]);

            await connectorClient.Conversations.SendToConversationAsync(reply);
        }

        /// <summary>
        /// Handles the activity from the bot context
        /// </summary>
        /// <param name="activity">Activity from the bot context</param>
        /// <returns></returns>
        private async Task HandleActivity(Activity activity)
        {
            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded != null && activity.MembersAdded.Any())
                {
                    foreach (var newMember in activity.MembersAdded)
                    {
                        if (newMember.Id != activity.Recipient.Id)
                        {
                            await Conversation.SendAsync(activity, () => this.LifetimeScope.Resolve<RootDialog>());
                        }
                    }
                }
            }
            else if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => this.LifetimeScope.Resolve<RootDialog>());
            }
        }
    }
}