
using System;
using System.Security.AccessControl;
using UnityEngine.Events;

[Serializable]
public class PublisherUnityEvent<EventData> : UnityEvent<EventData>
{
    public event EventHandler<EventData> onInvoke;

    public void InvokeAll(object sender, EventData eventData)
    {
        onInvoke.Invoke(sender, eventData);
        base.Invoke(eventData);
    }

    public void Subscribe(EventHandler<EventData> subscriber)
    {
        onInvoke += subscriber;
    }

    public void Unsubscribe(EventHandler<EventData> subscriber)
    {
        onInvoke -= subscriber;
    }
}

[Serializable]
public class PublisherUnityEvent : UnityEvent
{
    public event EventHandler onInvoke;

    public void InvokeAll(object sender, EventArgs eventArgs)
    {
        onInvoke.Invoke(sender, eventArgs);
        base.Invoke();
    }

    public void InvokeAll(object sender)
    {
        onInvoke.Invoke(sender, EventArgs.Empty);
        base.Invoke();
    }

    public void Subscribe(EventHandler subscriber)
    {
        onInvoke += subscriber;
    }

    public void Unsubscribe(EventHandler subscriber)
    {
        onInvoke -= subscriber;
    }
}