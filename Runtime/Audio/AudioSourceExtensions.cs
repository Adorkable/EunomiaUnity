using Eunomia;
using System;
using UnityEngine;

public static class AudioSourceExtensions
{
    public static float[] GetSpectrumDataSafe(this AudioSource audioSource, int sampleRange, int channel = -1, FFTWindow spectrumDataWindow = FFTWindow.Rectangular)
    {
        if (audioSource == null)
        {
            // TODO: custom exception
            throw new Exception("No audio source available");
        }

        if (audioSource.clip == null)
        {
            // TODO: custom exception
            throw new Exception("No audio clip attached");
        }

        sampleRange = Mathf.ClosestPowerOfTwo(sampleRange);
        if (sampleRange is < 64 or > 8192)
        {
            throw new Exception("Invalid sample range, must be between 64 and 8192");
        }

        var data = new float[sampleRange];

        if (channel == -1)
        {
            var buffer = new float[data.Length];
            for (var channelIndex = 0; channelIndex < audioSource.clip.channels; channelIndex++)
            {
                try
                {
                    audioSource.GetSpectrumData(buffer, channelIndex, spectrumDataWindow);
                }
                catch (Exception exception)
                {
                    Debug.LogError("Sample Range: " + sampleRange + ", Channel: " + channelIndex);
                    Debug.LogException(exception);
                    continue;
                }

                for (var bufferIndex = 0; bufferIndex < data.Length; bufferIndex++)
                {
                    data[bufferIndex] += buffer[bufferIndex];
                }
            }

            for (var bufferIndex = 0; bufferIndex < data.Length; bufferIndex++)
            {
                data[bufferIndex] /= audioSource.clip.channels;
            }
        }
        else if (channel >= 0 && channel < audioSource.clip.channels)
        {
            try
            {
                audioSource.GetSpectrumData(data, channel, spectrumDataWindow);
            }
            catch (Exception exception)
            {
                Debug.LogError("Sample Range: " + sampleRange + ", Channel: " + channel);
                Debug.LogException(exception);
            }
        }

        return data;
    }

    public static float Frequency(this AudioSource audioSource, int sampleRange, int channel = -1, FFTWindow spectrumDataWindow = FFTWindow.Rectangular)
    {
        if (audioSource == null || audioSource.clip == null)
        {
            return 0;
        }

        try
        {
            var spectrum = audioSource.GetSpectrumDataSafe(sampleRange, channel, spectrumDataWindow);

            return spectrum.Reduce((previous, current) =>
            {
                return previous + current;
            }, 0.0f) / spectrum.Length;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            return 0;
        }
    }

    public static float CurrentAudioClipLoudness(this AudioSource audioSource, int sampleRangeBack, int channel = -1)
    {
        if (audioSource.clip == null)
        {
            // TODO: throw?
            return 0;
        }
        var currentPosition = audioSource.timeSamples;
        return audioSource.clip.Loudness(currentPosition - sampleRangeBack, currentPosition, channel);
    }
}
