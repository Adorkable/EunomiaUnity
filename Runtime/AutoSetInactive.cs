using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    // TODO: combine with OnFinished for On...
    public class AutoSetInactive : MonoBehaviour
    {
        public enum When
        {
            Awake,
            Start,
            Update
        }

        [SerializeField] private When when;

        private void Awake()
        {
            if (when == When.Awake)
            {
                gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (when == When.Start)
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (when == When.Update)
            {
                gameObject.SetActive(false);
            }
        }
    }
}