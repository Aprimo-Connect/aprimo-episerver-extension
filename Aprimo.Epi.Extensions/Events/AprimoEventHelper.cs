using System;

namespace Aprimo.Epi.Extensions.Events
{
    internal static class AprimoEventHelper
    {
        public static void Trigger(object sender, EventArgs args, params EventHandler[] stages)
        {
            foreach (EventHandler stage in stages)
                if (stage != null)
                    stage.Invoke(sender, args);
        }

        public static void Trigger<T>(object sender, T args, params EventHandler<T>[] stages) where T : EventArgs
        {
            foreach (EventHandler<T> stage in stages)
                if (stage != null)
                    stage.Invoke(sender, args);
        }

        public static void TriggerCancelable<T>(object sender, T args, params EventHandler<T>[] stages) where T : EventArgs, ICancelable
        {
            foreach (EventHandler<T> stage in stages)
            {
                if (stage != null)
                {
                    foreach (EventHandler<T> invocation in stage.GetInvocationList())
                    {
                        invocation.Invoke(sender, args);

                        if (args.Cancelled)
                            return;
                    }
                }
            }
        }
    }
}