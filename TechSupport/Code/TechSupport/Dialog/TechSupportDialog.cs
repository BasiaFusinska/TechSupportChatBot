using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            await context.PostAsync("Hello IT?");
            context.Wait(MessageReceived);
        }
    }
}