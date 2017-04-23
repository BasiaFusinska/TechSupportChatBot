using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using ITCrowd.Controllers;

namespace ITCrowd
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                // return our reply to the user
                Activity reply = activity.CreateReply(GetAnswer(activity.Text, userData));
                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private string GetAnswer(string message, BotData userData)
        {
            const string STAGE = "conversationStage";
            var conversationStage = userData.GetProperty<ConversationStage>(STAGE);
            switch (conversationStage)
            {
                case ConversationStage.Hello:
                    userData.SetProperty(STAGE, ConversationStage.SomethingWrong);
                    return "Hello IT?";
                case ConversationStage.SomethingWrong:
                    userData.SetProperty(STAGE, ConversationStage.TurnOnOff);
                    return "Have you tried to turn it off an on again?";
                default:
                    userData.SetProperty(STAGE, ConversationStage.Hello);
                    return "You are welcome then";
            }
        }
    }
}