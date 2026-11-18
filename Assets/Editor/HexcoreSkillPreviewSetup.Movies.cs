using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEditor;
using UnityEditor.Media;
using UnityEngine;

public static partial class HexcoreSkillPreviewSetup
{
    private const string MovieRoot = "Library/HexcoreVisualMovies";
    private const int MovieFps = 30;
    private const int MovieWidth = 640;
    private const int MovieHeight = 360;
    private static readonly (int form, int skill, int heading, string label)[] MovieVariants =
    {
        (2, 12, 2, "R1-Thien-Van-Tram"),
        (3, 13, 1, "R2-Tam-Thien-Kiem-Quyet"),
        (1, 14, 3, "R3-Van-Anh-Tuyet-Sat")
    };
    private static readonly List<string> Movies = new();
    private static HexcorePreviewMixCapture mixCapture;
    private static double movieStart;
    private static int movieIndex;
    private static int movieFrame;
    private static bool movieRecording;

    private static bool RecordCoreMovies(HexcoreSkillPreview preview)
    {
        if (movieIndex >= MovieVariants.Length * 2) return true;
        var variant = MovieVariants[movieIndex / 2];
        bool slow = movieIndex % 2 == 1;
        string name = variant.label + (slow ? "-05x" : "-1x");
        var camera = UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (!movieRecording)
        {
            if (mixCapture == null) mixCapture = camera.gameObject.AddComponent<HexcorePreviewMixCapture>();
            mixCapture.Begin();
            preview.SelectSkill(variant.form, variant.skill, 2, variant.heading, false, slow);
            movieStart = EditorApplication.timeSinceStartup;
            movieFrame = 0;
            movieRecording = true;
        }
        float duration = HexcoreSkillPreview.SkillDuration(variant.skill) * (slow ? 2 : 1) + .55f;
        float time = (float)(EditorApplication.timeSinceStartup - movieStart);
        int target = Mathf.Min(Mathf.FloorToInt(time * MovieFps), Mathf.CeilToInt(duration * MovieFps) - 1);
        while (movieFrame <= target)
        {
            CaptureFrame(camera, $"{MovieRoot}/Frames/{name}", $"{movieFrame:000}", MovieWidth, MovieHeight);
            movieFrame++;
        }
        if (time < duration) return false;

        var samples = mixCapture.End(out int channels);
        int sampleRate = AudioSettings.outputSampleRate;
        try { EncodeCoreMovie(name, samples, channels, sampleRate); }
        catch (Exception exception) { Debug.LogWarning($"HEXCORE movie {name}: {exception.Message}"); }
        movieRecording = false;
        movieIndex++;
        if (movieIndex == MovieVariants.Length * 2)
        {
            Movies.Insert(0, "movie,frames,sample_rate,channels,peak");
            File.WriteAllLines($"{MovieRoot}/Movies.csv", Movies);
        }
        return false;
    }

    private static void EncodeCoreMovie(string name, float[] samples, int channels, int rate)
    {
        float peak = 0;
        foreach (float sample in samples) peak = Mathf.Max(peak, Mathf.Abs(sample));
        bool hasSound = peak > .0001f && channels is 1 or 2;
        var video = new VideoTrackAttributes
        {
            width = MovieWidth, height = MovieHeight, frameRate = new MediaRational(MovieFps)
        };
        var audio = new AudioTrackAttributes
        {
            sampleRate = new MediaRational(rate), channelCount = (ushort)channels, language = "vi"
        };
        string path = $"{MovieRoot}/{name}.mp4";
        using var encoder = hasSound ? new MediaEncoder(path, video, audio) : new MediaEncoder(path, video);
        var block = new NativeArray<float>(hasSound ? rate * channels / MovieFps : 0, Allocator.Temp);
        var source = new Texture2D(MovieWidth, MovieHeight, TextureFormat.RGB24, false);
        var frame = new Texture2D(MovieWidth, MovieHeight, TextureFormat.RGBA32, false);
        try
        {
            for (int i = 0; i < movieFrame; i++)
            {
                if (!source.LoadImage(File.ReadAllBytes($"{MovieRoot}/Frames/{name}/{i:000}.png")))
                    throw new IOException("Could not decode capture frame.");
                frame.SetPixels32(source.GetPixels32());
                frame.Apply();
                if (!encoder.AddFrame(frame)) throw new IOException("Video frame rejected.");
                if (!hasSound) continue;
                for (int j = 0; j < block.Length; j++)
                {
                    int offset = i * block.Length + j;
                    block[j] = offset < samples.Length ? samples[offset] : 0;
                }
                if (!encoder.AddSamples(block)) throw new IOException("Audio block rejected.");
            }
        }
        finally
        {
            block.Dispose();
            UnityEngine.Object.DestroyImmediate(source);
            UnityEngine.Object.DestroyImmediate(frame);
        }
        Movies.Add(FormattableString.Invariant($"{name},{movieFrame},{rate},{(hasSound ? channels : 0)},{peak:F6}"));
        Debug.Log($"HEXCORE MOVIE {name}: {movieFrame} frames, audio peak {peak:F4}.");
    }

    private static void ResetMovieCapture()
    {
        movieIndex = 0;
        movieRecording = false;
        movieFrame = 0;
        mixCapture = null;
        Movies.Clear();
    }
}
