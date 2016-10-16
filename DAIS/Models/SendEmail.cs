using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DAIS.Models
{
    [Serializable]
    public class SendEmail
    {
        public SendEmail(string sendTo)
        {
            SendTo = sendTo;
        }

        [Prompt("Please enter an Email address to sent it to {||}")]
        public string SendTo;
        [Prompt("What is a subject of the email? {||}")]
        public string Subject;
        [Prompt("Please type a body of your email {||}")]
        public string Body;
        [Prompt("How we can contact you back? {||}")]
        public string Contact;
    }
}
