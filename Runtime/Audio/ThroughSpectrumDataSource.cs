using UnityEngine;

namespace EunomiaUnity
{
    public class ThroughSpectrumDataSource : SpectrumDataSource
    {
        [SerializeField]
        private SpectrumDataSource input;

        public override float[] SpectrumData()
        {
            return input.SpectrumData();
        }
    }
}