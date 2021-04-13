using UnityEngine;

namespace EunomiaUnity
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 degreesPerSecond;

        void Update()
        {
            transform.eulerAngles = transform.eulerAngles + degreesPerSecond * Time.deltaTime;
        }
    }
}
