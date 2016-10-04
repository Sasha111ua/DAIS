using Autofac;
using DAIS.Dialogs;
using DAIS.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Sample.AlarmBot.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Bot.Sample.AlarmBot.Models
{
    /// <summary>
    /// These are the services (and their dependency structure) for the alarm sample.
    /// </summary>
    public sealed class AlarmModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new LuisModelAttribute("df72f0ca-5c49-499a-b5f3-3111899c7c5c", "f14c341ba26e4194a83240c92a5a2a0f")).AsSelf().AsImplementedInterfaces().SingleInstance();

            // register the top level dialog
            builder.RegisterType<DAISLuisDialog>().As<IDialog<object>>().InstancePerDependency();

            // register other dialogs we use
            builder.Register((c, p) => new AlarmRingDialog(p.TypedAs<string>(), c.Resolve<IAlarmService>(), c.Resolve<IAlarmRenderer>())).AsSelf().InstancePerDependency();

            // register some singleton services
            builder.RegisterType<SystemClock>().Keyed<IClock>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<LuisService>().Keyed<ILuisService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ResolutionParser>().Keyed<IResolutionParser>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<WesternCalendarPlus>().Keyed<ICalendarPlus>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StrictEntityToType>().Keyed<IEntityToType>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<NaiveAlarmScheduler>().Keyed<IAlarmScheduler>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ResponseService>().Keyed<IResponseService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();

            // register some objects dependent on the incoming message
            builder.Register(c => new RenderingAlarmService(new AlarmService(c.Resolve<IAlarmScheduler>(), c.Resolve<IBotToUser>(), c.Resolve<ResumptionCookie>()), c.Resolve<Func<IAlarmRenderer>>(), c.Resolve<IBotToUser>(), c.Resolve<IClock>())).Keyed<IAlarmService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
            builder.RegisterType<AlarmScorable>().Keyed<IScorable<double>>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
            builder.RegisterType<AlarmRenderer>().Keyed<IAlarmRenderer>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
        }
    }
}