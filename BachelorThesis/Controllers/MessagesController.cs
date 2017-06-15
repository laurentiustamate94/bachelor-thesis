using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using BachelorThesis.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BachelorThesis
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public ILifetimeScope LifetimeScope { get; set; }

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
                //TODO Log this
                return base.Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
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