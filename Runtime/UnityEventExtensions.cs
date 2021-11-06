using System.Reflection;
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
    }
}