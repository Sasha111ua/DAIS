using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;
using Microsoft.Bot.Sample.AlarmBot.Models;
using DAIS.Services;
using Microsoft.Bot.Builder.FormFlow;
using DAIS.Models;

namespace DAIS.Dialogs
{
    /// <summary>
    /// Entities for the built-in alarm LUIS model.
    /// </summary>
    public static partial class BuiltIn
    {
        public static partial class Alarm
        {
            public const string Alarm_State = "builtin.alarm.alarm_state";
            public const string Duration = "builtin.alarm.duration";
            public const string Start_Date = "builtin.alarm.start_date";
            public const string Start_Time = "builtin.alarm.start_time";
            public const string Title = "builtin.alarm.title";
        }

        public static class Entities
        {
            public static string InformationType = "InformationType";

            public static string Language = "Language";

            public static string Technology = "technology";
        }
    }

    /// <summary>
    /// The top-level natural language dialog for the alarm sample.
    /// </summary>
    [Serializable]
    public class DAISLuisDialog : LuisDialog<object>
    {
        private readonly IAlarmService service;
        private readonly IEntityToType entityToType;
        private readonly IClock clock;
        private readonly IResponseService responseService;

        public DAISLuisDialog(IAlarmService service, IResponseService responseService, IEntityToType entityToType, ILuisService luis, IClock clock)
            : base(luis)
        {
            SetField.NotNull(out this.service, nameof(service), service);
            SetField.NotNull(out this.entityToType, nameof(entityToType), entityToType);
            SetField.NotNull(out this.clock, nameof(clock), clock);
            SetField.NotNull(out this.responseService, nameof(responseService), responseService);

        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        public bool TryFindTitle(LuisResult result, out string title)
        {
            EntityRecommendation entity;
            if (result.TryFindEntity(BuiltIn.Alarm.Title, out entity))
            {
                title = entity.Entity;
                return true;
            }

            title = null;
            return false;
        }

        [LuisIntent("builtin.intent.alarm.delete_alarm")]
        public async Task DeleteAlarm(IDialogContext context, LuisResult result)
        {
            string title;
            TryFindTitle(result, out title);
            try
            {
                await this.service.DeleteAsync(title);
            }
            catch (AlarmNotFoundException)
            {
                await context.PostAsync("did not find alarm");
            }

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
                return;
            }

            if (sendEmail != null)
            {
                await context.PostAsync("Your Email has been sent");
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }

            context.Wait(MessageReceived);
        }

        private static IForm<SendEmail> BuildForm()
        {
            var builder = new FormBuilder<SendEmail>();

            //ActiveDelegate<SendEmail> isBYO = (email) => pizza. == PizzaOptions.BYOPizza;
            //ActiveDelegate<SendEmail> isSignature = (pizza) => pizza.Kind == PizzaOptions.SignaturePizza;
            //ActiveDelegate<SendEmail> isGourmet = (pizza) => pizza.Kind == PizzaOptions.GourmetDelitePizza;
            //ActiveDelegate<SendEmail> isStuffed = (pizza) => pizza.Kind == PizzaOptions.StuffedPizza;

            return builder
                // .Field(nameof(PizzaOrder.Choice))
                .Field(nameof(SendEmail.SendTo))
                .Field(nameof(SendEmail.Subject))
                .Field(nameof(SendEmail.Body))
                .AddRemainingFields()
                //.Confirm("Would you like a {Size}, {BYO.Crust} crust, {BYO.Sauce}, {BYO.Toppings} pizza?", isBYO)
                //.Confirm("Would you like a {Size}, {&Signature} {Signature} pizza?", isSignature, dependencies: new string[] { "Size", "Kind", "Signature" })
                //.Confirm("Would you like a {Size}, {&GourmetDelite} {GourmetDelite} pizza?", isGourmet)
                //.Confirm("Would you like a {Size}, {&Stuffed} {Stuffed} pizza?", isStuffed)
                .Build()
                ;
        }

        [LuisIntent("builtin.intent.alarm.find_alarm")]
        public async Task FindAlarm(IDialogContext context, LuisResult result)
        {
            string title;
            TryFindTitle(result, out title);
            try
            {
                await this.service.UpsertAsync(title, null, null);
            }
            catch (AlarmNotFoundException)
            {
                await context.PostAsync("did not find alarm");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.alarm.set_alarm")]
        public async Task SetAlarm(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            string title;
            bool? state = null;
            DateTime? when = null;

            TryFindTitle(result, out title);

            EntityRecommendation entity;
            if (result.TryFindEntity(BuiltIn.Alarm.Alarm_State, out entity))
            {
                state = entity.Entity.Equals("on", StringComparison.InvariantCultureIgnoreCase);
            }

            var now = this.clock.Now;

            IEnumerable<Range<DateTime>> ranges;
            if (entityToType.TryMapToDateRanges(now, result.Entities, out ranges))
            {
                using (var enumerator = ranges.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        when = enumerator.Current.Start;
                    }
                }
            }

            await this.service.UpsertAsync(title, when, state);

            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.alarm.snooze")]
        public async Task AlarmSnooze(IDialogContext context, LuisResult result)
        {
            string title;
            TryFindTitle(result, out title);
            try
            {
                await this.service.SnoozeAsync(title);
            }
            catch (AlarmNotFoundException)
            {
                await context.PostAsync("did not find alarm");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.alarm.turn_off_alarm")]
        public async Task TurnOffAlarm(IDialogContext context, LuisResult result)
        {
            string title;
            TryFindTitle(result, out title);
            try
            {
                await this.service.UpsertAsync(title, null, state: false);
            }
            catch (AlarmNotFoundException)
            {
                await context.PostAsync("did not find alarm");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.alarm.time_remaining")]
        [LuisIntent("builtin.intent.alarm.alarm_other")]
        public async Task AlarmOther(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I don't know how to handle that.");
            context.Wait(MessageReceived);
        }
    }
}