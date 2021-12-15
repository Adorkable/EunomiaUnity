using System;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    /// <summary>
    /// Accepts a list of GameObjects in order of priority. The first non-null GameObject that is Active is kept as is, all following GameObjects are set as Inactive. If no GameObjects are Active the last GameObject in the list is made Active
    /// </summary>
    [Serializable]
    public class ExclusiveFallbackActivator : AutoProcessor
    {
        [SerializeField]
        private GameObject[] fallbackOrder;

        public override bool HasStarted => hasStarted;
        private bool hasStarted = false;

        public override bool HasFinished => hasFinished;
        private bool hasFinished = false;

        protected override void AutoProcess()
        {
            hasStarted = true;
            SetExclusiveActive();
            hasFinished = true;
        }

        void SetExclusiveActive()
        {
            if (fallbackOrder == null || !fallbackOrder.Any())
            {
                Debug.LogError($"{nameof(fallbackOrder)} is empty or null", this);
                return;
            }

            GameObject lastNotNull = null;
            var foundActive = false;
            for (var index = 0; index < fallbackOrder.Count(); index++)
            {
                var current = fallbackOrder[index];
                if (current != null)
                {
                    lastNotNull = current;
                }
                if (!foundActive)
                {
                    if (current == null)
                    {
                        continue;
                    }
                    if (current.activeSelf)
                    {
                        foundActive = true;
                        continue;
                    }
                }
                else
                {
                    if (current == null)
                    {
                        continue;
                    }
                    current.SetActive(false);
                }
            }

            if (!foundActive)
            {
                if (lastNotNull != null)
                {
                    lastNotNull.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"All {nameof(GameObject)}s in {nameof(fallbackOrder)} are null, nothing set active", this);
                }
            }
        }
    }
}