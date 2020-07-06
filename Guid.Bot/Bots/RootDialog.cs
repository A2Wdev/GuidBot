using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Guid.Bot
{
    public class RootDialog : ComponentDialog
    {
        private static log4net.ILog logger
            = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string Hi = "Say (Hi)";
        private const string Welcome = "Say (Welcome)";
        private const string Hollo = "Say (Hollo)";
        private const string GreetMessage
            = "Welcome to **Riyadh Chat Bot**.\n\nI am designed to use with mobile email app, " +
            "make sure your replies do not contain signatures. \n\nFollowing is what I can help you with, " +
            "just reply with word in parenthesis:";
        private const string ErrorMessage = "Not a valid option";

        private static List<Choice> HelpdeskOptions = new List<Choice>()
            {
                new Choice(Hi) { Synonyms = new List<string> { "Hi" } },
                new Choice(Welcome) { Synonyms = new List<string> { "Welcome" } },
                new Choice(Hollo)  { Synonyms = new List<string> { "Hollo" } }
            };

        public RootDialog()
            : base(nameof(RootDialog))
        {
            InitialDialogId = nameof(WaterfallDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                PromptForOptionsAsync,
                ShowChildDialogAsync,
                //ResumeAfterAsync,
            }));
           
            AddDialog(new ShopDailog());
           
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
        }

        private async Task<DialogTurnResult> PromptForOptionsAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Prompt the user for a response using our choice prompt.
            return await stepContext.PromptAsync(
                nameof(ChoicePrompt),
                new PromptOptions()
                {
                    Choices = HelpdeskOptions,
                    Prompt = MessageFactory.Text(GreetMessage),
                    RetryPrompt = MessageFactory.Text(ErrorMessage)
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ShowChildDialogAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            // string optionSelected = await userReply;
            var optionSelected = (stepContext.Result as FoundChoice).Value;

            switch (optionSelected)
            {
                case Hi:

                    await stepContext.Context.SendActivityAsync("Hi That is Bot With .Net Core.", cancellationToken: cancellationToken);
                    return await stepContext.NextAsync(cancellationToken: cancellationToken);
                //context.Call(new InstallAppDialog(), this.ResumeAfterOptionDialog);
                //break;

                //return await stepContext.BeginDialogAsync(
                //    nameof(ShopDailog),
                //    cancellationToken);
                case Welcome:
                    //context.Call(new ResetPasswordDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    await stepContext.Context.SendActivityAsync("Welcome To Bot With .Net Core.", cancellationToken: cancellationToken);
                    return await stepContext.NextAsync(cancellationToken: cancellationToken);
                //return await stepContext.BeginDialogAsync(
                //    nameof(ShopDailog),
                //    cancellationToken);
                case Hollo:
                    //context.Call(new LocalAdminDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    await stepContext.Context.SendActivityAsync("Hollo That is Bot With .Net Core.", cancellationToken: cancellationToken);
                    return await stepContext.NextAsync(cancellationToken: cancellationToken);
                    //return await stepContext.BeginDialogAsync(
                    //    nameof(ShopDailog),
                    //    cancellationToken);
            }

            // We shouldn't get here, but fail gracefully if we do.
            await stepContext.Context.SendActivityAsync(
                "I don't recognize that option.",
                cancellationToken: cancellationToken);
            // Continue through to the next step without starting a child dialog.
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> ResumeAfterAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                //var message = await userReply;
                var message = stepContext.Context.Activity;

                var ticketNumber = new Random().Next(0, 20000);
                //await context.PostAsync($"Thank you for using the Helpdesk Bot. Your ticket number is {ticketNumber}.");
                await stepContext.Context.SendActivityAsync(
                    $"Thank you for using the Helpdesk Bot. Your ticket number is {ticketNumber}.",
                    cancellationToken: cancellationToken);

                //context.Done(ticketNumber);
            }
            catch (Exception ex)
            {
                // await context.PostAsync($"Failed with message: {ex.Message}");
                await stepContext.Context.SendActivityAsync(
                    $"Failed with message: {ex.Message}",
                    cancellationToken: cancellationToken);

                // In general resume from task after calling a child dialog is a good place to handle exceptions
                // try catch will capture exceptions from the bot framework awaitable object which is essentially "userReply"
                logger.Error(ex);
            }

            // Replace on the stack the current instance of the waterfall with a new instance,
            // and start from the top.
            return await stepContext.ReplaceDialogAsync(
                nameof(WaterfallDialog),
                cancellationToken: cancellationToken);
        }
    }
}
