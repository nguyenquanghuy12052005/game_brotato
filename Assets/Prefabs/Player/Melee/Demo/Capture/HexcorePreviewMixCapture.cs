using System.Collections.Generic;
using UnityEngine;

// Added temporarily to the preview listener by the editor capture runner.
public sealed class HexcorePreviewMixCapture : MonoBehaviour
{
    private readonly List<float> mix = new();
    private bool recording;
    private int channels;

    public void Begin()
    {
        lock (mix)
        {
            mix.Clear();
            channels = 0;
            recording = true;
        }
    }

    public float[] End(out int channelCount)
    {
        lock (mix)
        {
            recording = false;
            channelCount = channels;
            return mix.ToArray();
        }
    }

    private void OnAudioFilterRead(float[] data, int channelCount)
    {
        lock (mix)
        {
            if (!recording) return;
            if (channels != 0 && channels != channelCount) return;
            channels = channelCount;
            mix.AddRange(data);
        }
    }
}
