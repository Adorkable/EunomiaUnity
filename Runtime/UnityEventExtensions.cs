using System.Reflection;
using UnityEngine.Events;

namespace EunomiaUnity
{
    public static class UnityEventExtensions
    {
        private static FieldInfo CallsField
        {
            get
            {
                return typeof(UnityEventBase).GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            }
        }

        private static int GetInvokableCallListCount(this UnityEventBase unityEvent, FieldInfo fieldInfo)
        {
            var invokableCallList = CallsField.GetValue(unityEvent);
            var property = invokableCallList.GetType().GetProperty("Count");
            return (int)property.GetValue(invokableCallList);
        }

        private static int GetCallsCount(this UnityEventBase unityEvent)
        {
            return unityEvent.GetInvokableCallListCount(CallsField);
        }

        private static FieldInfo PersistentCallsField
        {
            get
            {
                return typeof(UnityEventBase).GetField("m_PersistentCalls", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            }
        }

        private static int GetPersistentCallGroupCount(this UnityEventBase unityEvent, FieldInfo fieldInfo)
        {
            var invokableCallList = fieldInfo.GetValue(unityEvent);
            var property = invokableCallList.GetType().GetProperty("Count");
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
