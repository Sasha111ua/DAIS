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
        //[Template(TemplateUsage.NotUnderstood, "What does \"{0}\" mean???")]
        //[Describe("Kind of pizza")]
        public string Subject;
        [Prompt("Please type a body of your email {||}")]
        public string Body;
        [Prompt("How we can contact you back? {||}")]
        public string Contact;

        //public string Address;
        //[Optional]
        //public CouponOptions Coupon;

        //public override string ToString()
        //{
        //    var builder = new StringBuilder();
        //    builder.AppendFormat("PizzaOrder({0}, ", Size);
        //    switch (Kind)
        //    {
        //        case PizzaOptions.BYOPizza:
        //            builder.AppendFormat("{0}, {1}, {2}, [", Kind, BYO.Crust, BYO.Sauce);
        //            foreach (var topping in BYO.Toppings)
        //            {
        //                builder.AppendFormat("{0} ", topping);
        //            }
        //            builder.AppendFormat("]");
        //            break;
        //        case PizzaOptions.GourmetDelitePizza:
        //            builder.AppendFormat("{0}, {1}", Kind, GourmetDelite);
        //            break;
        //        case PizzaOptions.SignaturePizza:
        //            builder.AppendFormat("{0}, {1}", Kind, Signature);
        //            break;
        //        case PizzaOptions.StuffedPizza:
        //            builder.AppendFormat("{0}, {1}", Kind, Stuffed);
        //            break;
        //    }
        //    builder.AppendFormat(", {0}, {1})", Address, Coupon);
        //    return builder.ToString();
        //}
    }
}
