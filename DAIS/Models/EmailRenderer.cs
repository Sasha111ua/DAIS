using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Bot.DAIS.Models
{
    public interface IEmailRenderer
    {
        Task RenderAsync(IBotToUser botToUser, string title, string body);
    }

    [Serializable]
    public sealed class EmailRenderer : IEmailRenderer
    {
        private readonly IEmailActions actions;
        public EmailRenderer(IEmailActions actions)
        {
            SetField.NotNull(out this.actions, nameof(actions), actions);
        }
        async Task IEmailRenderer.RenderAsync(IBotToUser botToUser, string title, string body)
        {
            Email email = new Email();
                var card = new HeroCard();
                card.Title = title;
                card.Subtitle = body;

                var buttons = this.actions.ActionsFor(email);
                card.Buttons = buttons.ToArray();

                var message = botToUser.MakeMessage();
                message.Attachments = new[] { card.ToAttachment() };

                await botToUser.PostAsync(message);
        }
    }
}