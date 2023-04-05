using System;
using UnityEngine;

public static class AudioClipExtensions
{
    public static float[] GetDataInSampleRange(this AudioClip audioClip, int startPosition, int endPosition)
    {
        if (startPosition < 0 || endPosition >= audioClip.samples)
        {
            // TODO: custom exception
            throw new Exception("Invalid start or end position");
        }

        var sampleRange = endPosition - startPosition;
        if (sampleRange <= 0)
        {
            // TODO: custom exception
            throw new Exception("Invalid start or end position");
        }

        float[] data = new float[sampleRange * audioClip.channels];
        audioClip.GetData(data, startPosition);
        return data;
    }

    public static float Loudness(this AudioClip audioClip, int startPosition, int endPosition, int channel = -1)
    {
        float[] data;
        try
        {
            data = audioClip.GetDataInSampleRange(startPosition, endPosition);
        }
        catch (Exception)
        {
            // TODO: throw up?
            return 0;
        }

        float sumLoudness = 0;
        for (var index = 0; index < data.Length / audioClip.channels; index++)
        {
            if (channel < 0)
            {
                for (var channelIndex = 0; channelIndex < audioClip.channels; channelIndex++)
                {
                    sumLoudness += Mathf.Abs(data[index * audioClip.channels + channelIndex]);
                }
            }
            else
            {
                if (channel >= audioClip.channels)
                {
                    return 0;
                }
                sumLoudness += Mathf.Abs(data[index * audioClip.channels]);
            }
        }

        return sumLoudness / (data.Length / audioClip.channels);
    }

    public static float Frequency(this AudioClip audioClip, int startPosition, int endPosition, int channel = -1)
    {
        float[] data;
        try
        {
            data = audioClip.GetDataInSampleRange(startPosition, endPosition);
        }
        catch (Exception exception)
        {
            // TODO: rethrow?
            Debug.LogError(exception);
            return 0;
        }

        var buffer = AudioClip.Create("buffer", data.Length, audioClip.channels, 1000, false);
        buffer.SetData(data, 0);
        return buffer.frequency;
    }
}
