using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class UnityEventExtensions
    {
        private static FieldInfo CallsField
        {
            get => typeof(UnityEventBase).GetField("m_Calls",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        }

        private static FieldInfo PersistentCallsField
        {
            get => typeof(UnityEventBase).GetField("m_PersistentCalls",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        }

        private static int GetInvokableCallListCount(this UnityEventBase unityEvent, FieldInfo fieldInfo)
        {
            var invokableCallList = CallsField.GetValue(unityEvent);
            var property = invokableCallList.GetType().GetProperty("Count");
            if (property == null)
            {
                return 0;
            }

            return (int)property.GetValue(invokableCallList);
        }

        private static int GetCallsCount(this UnityEventBase unityEvent)
        {
            return unityEvent.GetInvokableCallListCount(CallsField);
        }

        private static int GetPersistentCallGroupCount(this UnityEventBase unityEvent, FieldInfo fieldInfo)
        {
            var invokableCallList = fieldInfo.GetValue(unityEvent);
            var property = invokableCallList.GetType().GetProperty("Count");
            if (property == null)
            {
                return 0;
            }

            return (int)property.GetValue(invokableCallList);
        }

        private static int GetPersistentCallsCount(this UnityEventBase unityEvent)
        {
            return unityEvent.GetPersistentCallGroupCount(PersistentCallsField);
        }

        public static int GetAllListenersCount(this UnityEventBase unityEvent)
        {
            return unityEvent.GetCallsCount() + unityEvent.GetPersistentEventCount();
        }

        public static void InvokeAllCollectingExceptions(this UnityEventBase unityEvent, Action<UnityEngine.Object, string> invoker)
        {
            var exceptions = new List<Exception>();
            for (var index = 0; index < unityEvent.GetPersistentEventCount(); index++)
            {

                var target = unityEvent.GetPersistentTarget(index);
                var methodName = unityEvent.GetPersistentMethodName(index);

                try
                {
                    invoker(target, methodName);
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

        // TODO: thoroughly test
        public static void InvokeAllCollectingExceptions(this UnityEventBase unityEvent)
        {
            InvokeAllCollectingExceptions(unityEvent, (target, methodName) =>
            {
                var methodInfo = target.GetType().GetMethod(methodName);
                if (methodInfo == null)
                {
                    throw new ArgumentException($"Method {methodName} not found on {target.GetType().Name}");
                }

                methodInfo.Invoke(target, null);
            });
        }

        // TODO: thoroughly test
        public static void InvokeAllCollectingExceptions(this UnityEventBase unityEvent, EventArgs eventArgs)
        {
            InvokeAllCollectingExceptions(unityEvent, (target, methodName) =>
            {
                var methodInfo = target.GetType().GetMethod(methodName);
                if (methodInfo == null)
                {
                    throw new ArgumentException($"Method {methodName} not found on {target.GetType().Name}");
                }

                methodInfo.Invoke(target, new object[] { eventArgs });
            });
        }

        public static void InvokeAllCollectingExceptions<TEventData>(this UnityEvent<TEventData> unityEvent, TEventData eventData)
        {
            InvokeAllCollectingExceptions(unityEvent, (target, methodName) =>
            {
                var methodInfo = target.GetType().GetMethod(methodName);
                if (methodInfo == null)
                {
                    throw new ArgumentException($"Method {methodName} not found on {target.GetType().Name}");
                }

                methodInfo.Invoke(target, new object[] { eventData });
            });
        }
    }
}