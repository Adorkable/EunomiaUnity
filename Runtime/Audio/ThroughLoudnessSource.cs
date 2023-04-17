using UnityEngine;

namespace EunomiaUnity
{
    public class ThroughLoudnessSource : LoudnessSource
    {
        [SerializeField]
        private LoudnessSource input;

        public override float Loudness()
        {
            return input.Loudness();
        }
    }
}