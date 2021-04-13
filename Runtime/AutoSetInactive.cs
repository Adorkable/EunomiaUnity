using UnityEngine;

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
        [SerializeField]
        private When when;

        void Awake()
        {
            if (when == When.Awake)
            {
                gameObject.SetActive(false);
            }
        }

        void Start()
        {
            if (when == When.Start)
            {
                gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (when == When.Update)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
