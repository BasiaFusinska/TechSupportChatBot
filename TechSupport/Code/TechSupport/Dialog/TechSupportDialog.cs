using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TechSupport.Dialog
{
    [LuisModel("<modelID>", "<subscriptionKey>")]
    [Serializable]
    public class TechSupportDialog : LuisDialog<object>
    {
        private const string Issue_Entity = "BrokenItem";
        private const string Name_Entity = "EmployeeName";
        private const string Id_Entity = "EmployeeId";

        private DialogPhase dialogPhase = DialogPhase.Hello;

        private string[] issues;
        private string[] names;
        private string[] ids;

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
            ResetDialog();
            dialogPhase = DialogPhase.NotWorking;
            await Respond(context, Phrases.HELLO);
        }

        [LuisIntent("Bye")]
        public async Task Bye(IDialogContext context, LuisResult result)
        {
            dialogPhase = DialogPhase.Hello;
            await Respond(context, Phrases.GOODBYE);
        }

        [LuisIntent("NotWorking")]
        public async Task NotWorking(IDialogContext context, LuisResult result)
        {
            if (dialogPhase == DialogPhase.NotWorking)
            {
                issues = result.Entities.Where(e => e.Type == Issue_Entity).Select(e => e.Entity).ToArray();

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

            Thread.Sleep(400);
            await context.PostAsync(Phrases.REPAIR);

            names = result.Entities.Where(e => e.Type == Name_Entity).Select(e => e.Entity).ToArray();
            ids = result.Entities.Where(e => e.Type == Id_Entity).Select(e => e.Entity).ToArray();

            if (issues?.Length > 0 || names?.Length > 0 || ids?.Length > 0)
            {
                Thread.Sleep(400);
                var repairSummary = Phrases.REPAIR_SUMMARY + $"  {Environment.NewLine}" +
                    (issues?.Length > 0 ? string.Format(Phrases.ISSUE, issues[0]) + $"  {Environment.NewLine}" : "") +
                    (names?.Length > 0 ? string.Format(Phrases.NAME, string.Join(",", names).ToUpper()) + $"  {Environment.NewLine}" : "") +
                    (ids?.Length > 0 ? string.Format(Phrases.EMPLOYEE, ids[0].ToUpper()) : "");

                await context.PostAsync(repairSummary);
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Yes")]
        public async Task Yes(IDialogContext context, LuisResult result)
        {
            if (dialogPhase == DialogPhase.TurnOffOn)
            {
                dialogPhase = DialogPhase.Result;
                await Respond(context, Phrases.RESULT);
            }
            else
            {
                await Respond(context, Phrases.DIDNT_GET_THAT);
            }
        }

        [LuisIntent("No")]
        public async Task No(IDialogContext context, LuisResult result)
        {
            if (dialogPhase == DialogPhase.TurnOffOn)
            {
                dialogPhase = DialogPhase.Result;
                await Respond(context, Phrases.TURNOO_RESULT);
            }
            else
            {
                await Respond(context, Phrases.DIDNT_GET_THAT);
            }
        }

        [LuisIntent("Worked")]
        public async Task Worked(IDialogContext context, LuisResult result)
        {
            dialogPhase = DialogPhase.Hello;
            await Respond(context, Phrases.BYE);
        }

        [LuisIntent("Try")]
        public async Task Try(IDialogContext context, LuisResult result)
        {
            dialogPhase = DialogPhase.Result;
            await Respond(context, Phrases.RESULT);
        }

        private void ResetDialog()
        {
            issues = null;
            names = null;
            ids = null;
        }
    }
}