using System;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    [Serializable]
    public class PublisherUnityEvent<TEventData> : UnityEvent<TEventData>
    {
        public event EventHandler<TEventData> OnInvoke;

        public void InvokeAll(object sender, TEventData eventData)
        {
            OnInvoke?.Invoke(sender, eventData);

            Invoke(eventData);
        }

        public void Subscribe(EventHandler<TEventData> subscriber)
        {
            OnInvoke += subscriber;
        }

        public void Unsubscribe(EventHandler<TEventData> subscriber)
        {
            OnInvoke -= subscriber;
        }
    }

    [Serializable]
    public class PublisherUnityEvent : UnityEvent
    {
        public event EventHandler OnInvoke;

        public void InvokeAll(object sender, EventArgs eventArgs)
        {
            OnInvoke?.Invoke(sender, eventArgs);

            Invoke();
        }

        public void InvokeAll(object sender)
        {
            OnInvoke?.Invoke(sender, EventArgs.Empty);

            Invoke();
        }

        public void Subscribe(EventHandler subscriber)
        {
            OnInvoke += subscriber;
        }

        public void Unsubscribe(EventHandler subscriber)
        {
            OnInvoke -= subscriber;
        }
    }
}