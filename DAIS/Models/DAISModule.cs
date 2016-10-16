using Autofac;
using DAIS.Dialogs;
using DAIS.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using System;
using static Microsoft.Bot.DAIS.Models.EmailService;

namespace Microsoft.Bot.DAIS.Models
{
    public sealed class DAISModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new LuisModelAttribute("df72f0ca-5c49-499a-b5f3-3111899c7c5c", "f14c341ba26e4194a83240c92a5a2a0f")).AsSelf().AsImplementedInterfaces().SingleInstance();

            // register the top level dialog
            builder.RegisterType<DAISLuisDialog>().As<IDialog<object>>().InstancePerDependency();

            // register some singleton services
            builder.RegisterType<LuisService>().Keyed<ILuisService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ResolutionParser>().Keyed<IResolutionParser>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<WesternCalendarPlus>().Keyed<ICalendarPlus>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StrictEntityToType>().Keyed<IEntityToType>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ResponseService>().Keyed<IResponseService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();

            // register some objects dependent on the incoming message
            builder.Register(c => new RenderingDaisService(new EmailService(), c.Resolve<Func<IEmailRenderer>>(), c.Resolve<IBotToUser>())).Keyed<IEmailService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
            builder.RegisterType<EmailScorable>().Keyed<IScorable<double>>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
            builder.RegisterType<EmailRenderer>().Keyed<IEmailRenderer>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
        }
    }
}