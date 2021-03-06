﻿using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace Microsoft.Bot.DAIS.Models
{
    public interface IEmailActions
    {
        IEnumerable<CardAction> ActionsFor(Email email);
    }

    [Serializable]
    public sealed class EmailScorable : IEmailActions, IScorable<double>
    {
        private readonly IEmailService service;
        public EmailScorable(IEmailService service)
        {
            SetField.NotNull(out this.service, nameof(service), service);
        }

        public static class Verbs
        {
            public const string Send = "send";
            public const string Cancel = "cancel";
        }

        public static readonly IReadOnlyList<string> AllowedVerbs = typeof(Verbs).GetFields().Select(p => (string)p.GetValue(null)).ToArray();

        public const string Prefix = "button";
        public static string FormatValue(string verb, Email email)
        {
            return $"{Prefix}-{verb}-{email.Title}";
        }

        public bool TryParseValue(string value, out string verb, out string title)
        {
            if (value != null)
            {
                var parts = value.Split('-');
                if (parts.Length == 3)
                {
                    if (parts[0] == Prefix)
                    {
                        verb = parts[1];
                        title = parts[2];

                        if (AllowedVerbs.Contains(verb))
                        {
                            return true;
                        }
                    }
                }
            }

            verb = null;
            title = null;
            return false;
        }

        IEnumerable<CardAction> IEmailActions.ActionsFor(Email email)
        {
            Func<string, CardAction> ActionFor = verb =>
                new CardAction()
                {
                    Type = ActionTypes.ImBack,
                    Title = verb,
                    Value = FormatValue(verb, email)
                };

            yield return ActionFor(Verbs.Send);
            yield return ActionFor(Verbs.Cancel);
        }

        async Task<object> IScorable<double>.PrepareAsync<Item>(Item item, CancellationToken token)
        {
            var message = item as IMessageActivity;
            if (message != null && message.Text != null)
            {
                var text = message.Text;
                string verb;
                string title;
                if (TryParseValue(text, out verb, out title))
                {
                    return Tuple.Create(verb, title);
                }
            }

            return null;
        }

        bool IScorable<double>.TryScore(object state, out double score)
        {
            bool matched = state != null;
            score = matched ? 1.0 : double.NaN;
            return matched;
        }

        async Task IScorable<double>.PostAsync<Item>(Item item, object state, CancellationToken token)
        {
            var verbTitle = (Tuple<string, string>)state;
            var verb = verbTitle.Item1;
            var title = verbTitle.Item2;
            switch (verb)
            {
                case Verbs.Send:
                    await this.service.SendAsync(title);
                    
                    break;
                case Verbs.Cancel:
                    await this.service.CancelAsync(title);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}