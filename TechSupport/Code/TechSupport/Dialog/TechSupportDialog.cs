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
        private DialogPhase dialogPhase = DialogPhase.Hello;

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await Respond(context, Phrases.DIDNT_GET_THAT);
        }

        private async Task Respond(IDialogContext context, string message, int sleep = 400)
        {
            Thread.Sleep(sleep);
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Hello")]
        public async Task Hello(IDialogContext context, LuisResult result)
        {
            dialogPhase = DialogPhase.NotWorking;
            await Respond(context, Phrases.HELLO);
        }

        [LuisIntent("Bye")]
        public async Task Bye(IDialogContext context, LuisResult result)
        {
            await Respond(context, Phrases.GOODBYE);
        }

        [LuisIntent("NotWorking")]
        public async Task NotWorking(IDialogContext context, LuisResult result)
        {
            if (dialogPhase == DialogPhase.NotWorking)
            {
                dialogPhase = DialogPhase.TurnOffOn;
                await Respond(context, Phrases.TURN_OFF_ON);
            }
            else
            {
                dialogPhase = DialogPhase.Details;
                await Respond(context, Phrases.DETAILS);
            }
        }

        [LuisIntent("Details")]
        public async Task Details(IDialogContext context, LuisResult result)
        {
            dialogPhase = DialogPhase.Bye;
            await Respond(context, Phrases.REPAIR);
        }

    }
}