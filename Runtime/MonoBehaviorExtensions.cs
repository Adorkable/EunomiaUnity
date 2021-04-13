using System;
using UnityEngine;

namespace EunomiaUnity
{
    public static class MonoBehaviorExtensions
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
                LogMissingRequiredComponent(monoBehaviour, typeof(T));
            }
            return result;
        }

        public static void LogMissingRequiredComponent(this MonoBehaviour monoBehaviour, Type type)
        {
            Debug.LogError($"{monoBehaviour} requires {type} component to function correctly", monoBehaviour);
        }

        public static void LogMissingRequiredReference(this MonoBehaviour monoBehaviour, string type)
        {
            Debug.LogError($"{monoBehaviour} requires {type} reference to function correctly", monoBehaviour);
        }

        public static void LogMissingRequiredReference(this MonoBehaviour monoBehaviour, Type type)
        {
            LogMissingRequiredReference(monoBehaviour, type.ToString());
        }

        public static void LogMissingRequiredReference(this MonoBehaviour monoBehaviour, Type type, string name)
        {
            LogMissingRequiredReference(monoBehaviour, $"{name} of type {type}");
        }
    }
}
