using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using System;
using System.Threading.Tasks;

namespace Microsoft.Bot.DAIS.Models
{
    public interface IEmailService
    {
        Task UpsertAsync(string subject, string body);
        Task CancelAsync(string title);
        Task SendAsync(string title);
    }

    [Serializable]
    public sealed class EmailService : IEmailService
    {

        public EmailService()
        {
        }
        async Task IEmailService.CancelAsync(string title)
        {
        }
        async Task IEmailService.SendAsync(string title)
        {

        }
        async Task IEmailService.UpsertAsync(string subject, string body)
        {
            Email email;
            email = new Email() { Subject = subject, Body = body };
        }

        public sealed class RenderingDaisService : IEmailService
        {
            private readonly IEmailService inner;
            private readonly Func<IEmailRenderer> renderer;
            private readonly IBotToUser botToUser;
            public RenderingDaisService(IEmailService inner, Func<IEmailRenderer> renderer, IBotToUser botToUser)
            {
                SetField.NotNull(out this.inner, nameof(inner), inner);
                SetField.NotNull(out this.renderer, nameof(renderer), renderer);
                SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
            }
            async Task IEmailService.CancelAsync(string title)
            {
            }

            async Task IEmailService.SendAsync(string title)
            {
            }

            async Task IEmailService.UpsertAsync(string subject, string body)
            {
                await this.inner.UpsertAsync(subject, body);
                await this.renderer().RenderAsync(this.botToUser, subject, body);
            }
        }
    }
}