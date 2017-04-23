using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TechSupport.Dialog
{
    [LuisModel("<modelID>", "<subscriptionKey>")]
    [Serializable]
    public class TechSupportDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await DidntGetThat(context);
        }

        private async Task DidntGetThat(IDialogContext context)
        {
            await Respond(context, Phrases.DIDNT_GET_THAT);
        }

        [LuisIntent("Hello")]
        public async Task Hello(IDialogContext context, LuisResult result)
        {
            await Respond(context, Phrases.HELLO);
        }

        [LuisIntent("Bye")]
        public async Task Bye(IDialogContext context, LuisResult result)
        {
            await Respond(context, Phrases.GOODBYE);
        }

        private async Task Respond(IDialogContext context, string message, int sleep = 400)
        {
            Thread.Sleep(sleep);
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}