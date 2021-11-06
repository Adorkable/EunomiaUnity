using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 degreesPerSecond;

        private void Update()
        {
            transform.eulerAngles += degreesPerSecond * Time.deltaTime;
        }
    }
}