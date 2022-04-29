using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    [Serializable]
    public class PublisherUnityEvent<TEventData> : UnityEvent<TEventData>
    {
        public event EventHandler<TEventData> OnInvoke;

        // TODO: UnityEvent has some special editor hocus-pocus, figure out how to expose collectExceptions to Editor
        // [SerializeField, InfoBox("Enabling this will cause invoke all subscribers and throw all collected exceptions back at once. Disabing this will cause the first exception thrown to be thrown through and for no further subscribers to be invoked.")]
        // private bool collectExceptions = false;
        public bool collectExceptions = false;

        public void InvokeAll(object sender, TEventData eventData)
        {
            if (collectExceptions)
            {
                OnInvoke?.InvokeAllCollectingExceptions(sender, eventData);
                this.InvokeAllCollectingExceptions(eventData);
            }
            else
            {
                OnInvoke?.Invoke(sender, eventData);
                Invoke(eventData);
            }
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

        // TODO: UnityEvent has some special editor hocus-pocus, figure out how to expose collectExceptions to Editor
        // [SerializeField, InfoBox("Enabling this will cause invoke all subscribers and throw all collected exceptions back at once. Disabing this will cause the first exception thrown to be thrown through and for no further subscribers to be invoked.")]
        // private bool collectExceptions = false;
        public bool collectExceptions = false;

        public void InvokeAll(object sender, EventArgs eventData)
        {
            if (collectExceptions)
            {
                OnInvoke?.InvokeAllCollectingExceptions(sender, eventData);
                this.InvokeAllCollectingExceptions(eventData);
            }
            else
            {
                OnInvoke?.Invoke(sender, eventData);
                Invoke(new object[] { eventData });
            }
        }

        public void InvokeAll(object sender)
        {
            if (collectExceptions)
            {
                OnInvoke?.InvokeAllCollectingExceptions(sender, EventArgs.Empty);
                this.InvokeAllCollectingExceptions();
            }
            else
            {
                OnInvoke?.Invoke(sender, EventArgs.Empty);
                Invoke();
            }
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