using System;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;
using Microsoft.Bot.DAIS.Models;
using DAIS.Services;
using Microsoft.Bot.Builder.FormFlow;
using DAIS.Models;

namespace DAIS.Dialogs
{
    public static partial class BuiltIn
    {

        public static class Entities
        {
            public static string InformationType = "InformationType";

            public static string Language = "Language";

            public static string Technology = "technology";

            public static string BaseCurrency = "Base currency";

            public static string TargetCurrency = "target currency";

            public static string Ammount = "ammount";
        }
    }


    [Serializable]
    public class DAISLuisDialog : LuisDialog<object>
    {
        private readonly IEmailService service;
        private readonly IEntityToType entityToType;
        private readonly IResponseService responseService;

        public DAISLuisDialog(IEmailService service, IResponseService responseService, IEntityToType entityToType, ILuisService luis)
            : base(luis)
        {
            SetField.NotNull(out this.service, nameof(service), service);
            SetField.NotNull(out this.entityToType, nameof(entityToType), entityToType);
            SetField.NotNull(out this.responseService, nameof(responseService), responseService);

        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greetings")]
        public async Task RespondeToGreetings(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(responseService.CreateGreetingsResponce(result));
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task RespondeToNone(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(responseService.CreateNoneResponse(result));
            context.Wait(MessageReceived);
        }

        [LuisIntent("Company info")]
        public async Task RespondeToCompanyInfo(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(responseService.CreateCompanyInfoResponse(result));
            context.Wait(MessageReceived);
        }

        [LuisIntent("CurrencyConverter")]
        public async Task RespondeToCurrencyConverter(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(await responseService.CreateCompanyCurrencyConverter(result));
            context.Wait(MessageReceived);
        }

        [LuisIntent("Send email")]
        public async Task RespondeToSendEmail(IDialogContext context, LuisResult result)
        {
           var email = responseService.TryGetEmail(result);
            var form = new FormDialog<SendEmail>(
                     new SendEmail(email),
                     BuildForm,
                     FormOptions.PromptInStart,
                     result.Entities);

            context.Call(form, EmailFormComplete);
        }

        private async Task EmailFormComplete(IDialogContext context, IAwaitable<SendEmail> result)
        {

            SendEmail sendEmail = null;
            try
            {
                sendEmail = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the form!");
                context.Wait(MessageReceived);
                return;
            }

            if (sendEmail != null)
            {
                await this.service.UpsertAsync(sendEmail.Subject, sendEmail.Body);
                //await context.PostAsync("Your Email has been sent");
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
                context.Wait(MessageReceived);
            }

        }

        private static IForm<SendEmail> BuildForm()
        {
            var builder = new FormBuilder<SendEmail>();

            return builder
                .Field(nameof(SendEmail.SendTo))
                .Field(nameof(SendEmail.Subject))
                .Field(nameof(SendEmail.Body))
                .AddRemainingFields()
                .Build();
        }

    }
}