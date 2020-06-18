using System;
using System.Reflection;
using UnityEngine;

namespace EunomiaUnity {
    public static class ComponentExtensions {
        // Based on http://answers.unity.com/answers/641022/view.html
        public static T CopyProperties<T>(this Component component, T copyFrom) where T : Component {
            Type type = component.GetType();
            if (type != copyFrom.GetType()) { // type mis-match
                return null; 
            }
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] propertyInfos = type.GetProperties(flags);
            foreach (var propertyInfo in propertyInfos) {
                if (propertyInfo.CanWrite) {
                    try {
                        propertyInfo.SetValue(component, propertyInfo.GetValue(copyFrom, null), null);
                    } catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so not catching anything specific.
                }
            }
            FieldInfo[] fieldInfos = type.GetFields(flags);
            foreach (var fieldInfo in fieldInfos) {
                fieldInfo.SetValue(component, fieldInfo.GetValue(copyFrom));
            }
            return component as T;
        }
    }
}