using UnityEngine;

namespace EunomiaUnity
{
    public class ThroughFrequencySource : FrequencySource
    {
        [SerializeField]
        private FrequencySource input;

        public override float Frequency()
        {
            return input.Frequency();
        }
    }
}