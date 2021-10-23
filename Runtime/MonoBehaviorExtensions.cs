using System;
using UnityEngine;

namespace EunomiaUnity
{
    public static class MonoBehaviorExtensions_Require
    {
        public static T RequireInstance<T>(this MonoBehaviour monoBehaviour, T test) where T : Component
        {
            if (test != null)
            {
                return test;
            }

            return RequireComponentInstance<T>(monoBehaviour);
        }

        public static T RequireComponentInstance<T>(this MonoBehaviour monoBehaviour) where T : Component
        {
            var result = monoBehaviour.GetComponent<T>();
            if (result == null)
            {
                monoBehaviour.LogMissingRequiredComponent(typeof(T));
            }
            return result;
        }
    };

    public static class MonoBehaviorExtensions_LogMissingRequired
    {
        public static void LogMissingRequired(this MonoBehaviour monoBehaviour, string type, string context)
        {
            Debug.LogError($"{monoBehaviour} requires {type} {context} to function correctly", monoBehaviour);
        }

        public static void LogMissingRequired(this MonoBehaviour monoBehaviour, Type type, string context)
        {
            LogMissingRequired(monoBehaviour, type.ToString(), context);
        }

        public static void LogMissingRequiredComponent(this MonoBehaviour monoBehaviour, Type type)
        {
            LogMissingRequired(monoBehaviour, type, "component");
        }

        public static void LogMissingRequiredReference(this MonoBehaviour monoBehaviour, string type)
        {
            LogMissingRequired(monoBehaviour, type, "reference");
        }

        public static void LogMissingRequiredReference(this MonoBehaviour monoBehaviour, Type type)
        {
            LogMissingRequired(monoBehaviour, type, "reference");
        }

        public static void LogMissingRequiredReference(this MonoBehaviour monoBehaviour, Type type, string name)
        {
            LogMissingRequired(monoBehaviour, $"{name} of type {type}", "reference");
        }
    }
}
