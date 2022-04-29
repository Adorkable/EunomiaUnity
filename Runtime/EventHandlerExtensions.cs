// ReSharper disable once CheckNamespace
using System;
using System.Collections.Generic;

namespace EunomiaUnity
{
    public static class EventHandlerExtensions
    {
        public static void InvokeAllCollectingExceptions<TEventData>(this EventHandler<TEventData> OnInvoke, object sender, TEventData eventData)
        {
            var exceptions = new List<Exception>();
            foreach (var handler in OnInvoke.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(sender, eventData);
                }
                catch (Exception exception)
                {
                    exceptions.Add(exception);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        public static void InvokeAllCollectingExceptions(this EventHandler OnInvoke, object sender, EventArgs eventArgs)
        {
            var exceptions = new List<Exception>();
            foreach (var handler in OnInvoke.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(sender, eventArgs);
                }
                catch (Exception exception)
                {
                    exceptions.Add(exception);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}