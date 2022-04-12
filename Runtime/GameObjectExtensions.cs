using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class GameObjectExtensions
    {
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            foreach (var transform in gameObject.GetComponentsInChildren<Transform>(true))
            {
                transform.gameObject.layer = layer;
            }
        }

        public static T AddComponent<T>(this GameObject gameObject, T template) where T : Component
        {
            return gameObject.AddComponent<T>().CopyProperties(template);
        }

        public static void SetHideFlagsRecursively(this GameObject gameObject, HideFlags hideFlags)
        {
            gameObject.hideFlags = hideFlags;
            for (int index = 0; index < gameObject.transform.childCount; index++)
            {
                var child = gameObject.transform.GetChild(index);
                child.gameObject.SetHideFlagsRecursively(hideFlags);
            }
        }
    }
}