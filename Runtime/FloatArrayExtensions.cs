using UnityEngine;

public static class FloatArrayExtensions
{
    // Based on Simple Spectrum by Sam Boyer - http://samboyer.uk/
    public static float[] LogarithmicallyScaled(this float[] values)
    {
        var result = new float[values.Length];

        var highestLogSampleFreq = Mathf.Log(values.Length + 1, 2); //gets the highest possible logged frequency, used to calculate which sample of the spectrum to use for a bar

        var logSampleFreqMultiplier = values.Length / highestLogSampleFreq;

        for (var i = 0; i < values.Length; i++) //for each float in the output
        {
            var trueSampleIndex = (highestLogSampleFreq - Mathf.Log(values.Length + 1 - i, 2)) * logSampleFreqMultiplier; //gets the index equiv of the logified frequency

            //the true sample is usually a decimal, so we need to lerp between the floor and ceiling of it.

            var sampleIndexFloor = Mathf.FloorToInt(trueSampleIndex);
            sampleIndexFloor = Mathf.Clamp(sampleIndexFloor, 0, values.Length - 2); //just keeping it within the spectrum array's range

            var value = Mathf.SmoothStep(values[sampleIndexFloor], values[sampleIndexFloor + 1], trueSampleIndex - sampleIndexFloor); //smoothly interpolate between the two samples using the true index's decimal.

            value = value * (trueSampleIndex + 1); //multiply value by its position to make it proportionate
            value = Mathf.Sqrt(value); //compress the amplitude values by sqrt(x)

            result[i] = value;
        }
        return result;
    }
}
