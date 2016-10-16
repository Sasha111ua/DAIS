using System;

namespace Microsoft.Bot.DAIS.Models
{
    [Serializable]
    public sealed class Email
    {
        public string Title { get; set; }
        public string Subject { get; internal set; }
        public string Body { get; internal set; }
    }
}