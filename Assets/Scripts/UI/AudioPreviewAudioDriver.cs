using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public sealed class AudioPreviewAudioDriver : MonoBehaviour
{
    private AudioSource music, sfx, selected;
    private AudioClip loaded;
    private string loadedPath;
    private Coroutine loading;
    private int request;
    private float totalClock;
    private int lastSample;
    private bool wasPlaying;
    public bool Loading => loading != null;
    public bool Playing => selected != null && selected.isPlaying;
    public bool Finished { get; private set; }
    public float Position { get; private set; }
    public float TotalPosition => totalClock;
    public float Length => loaded != null ? loaded.length : 0;
    public string Problem { get; private set; }
    public string Group => selected == music ? "Preview / Music" : "Preview / SFX";

    private void Awake()
    {
        music = MakeBus("Preview Music");
        sfx = MakeBus("Preview SFX");
    }

    private AudioSource MakeBus(string label)
    {
        var child = new GameObject(label);
        child.transform.SetParent(transform, false);
        AudioSource source = child.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0;
        return source;
    }

    public void StartClip(string path, bool musicGroup, bool loop)
    {
        StopClip();
        Problem = null;
        selected = musicGroup ? music : sfx;
        if (path == null || !File.Exists(path))
        {
            Problem = "Thiếu WAV. Kiểm tra thư mục demos/brotato-audio.";
            return;
        }
        if (path == loadedPath && loaded != null)
        {
            Begin(loop);
            return;
        }
        if (loaded != null) Destroy(loaded);
        loaded = null;
        loadedPath = null;
        loading = StartCoroutine(LoadClip(path, loop, request));
    }

    private IEnumerator LoadClip(string path, bool loop, int token)
    {
        using var get = UnityWebRequestMultimedia.GetAudioClip(new Uri(path).AbsoluteUri, AudioType.WAV);
        yield return get.SendWebRequest();
        if (request != token) yield break;
        loading = null;
        if (get.result != UnityWebRequest.Result.Success)
        {
            Problem = "Unity không đọc được WAV · " + get.error;
            yield break;
        }
        loaded = DownloadHandlerAudioClip.GetContent(get);
        if (loaded == null)
        {
            Problem = "Unity không giải mã được WAV.";
            yield break;
        }
        loadedPath = path;
        Begin(loop);
    }

    private void Begin(bool loop)
    {
        selected.clip = loaded;
        selected.loop = loop;
        selected.Play();
        wasPlaying = true;
    }

    public void StopClip()
    {
        request++;
        if (loading != null) StopCoroutine(loading);
        loading = null;
        if (selected != null)
        {
            selected.Stop();
            selected.clip = null;
        }
        Position = totalClock = 0;
        lastSample = 0;
        Finished = wasPlaying = false;
        Problem = null;
    }

    public void SetMix(float master, float musicLevel, float sfxLevel, bool muteMusic, bool muteSfx, float speed, float pitch)
    {
        music.volume = master * musicLevel;
        sfx.volume = master * sfxLevel;
        music.mute = muteMusic;
        sfx.mute = muteSfx;
        music.pitch = sfx.pitch = Mathf.Clamp(speed * Mathf.Pow(2, pitch / 12f), .25f, 2f);
    }

    public void ReadClock()
    {
        if (!wasPlaying || loaded == null || selected == null) return;
        if (!selected.isPlaying)
        {
            wasPlaying = false;
            Finished = true;
            Position = loaded.length;
            return;
        }
        int sample = selected.timeSamples;
        int difference = sample - lastSample;
        if (selected.loop && difference < 0) difference += loaded.samples;
        if (difference > 0) totalClock += difference / (float)loaded.frequency;
        lastSample = sample;
        Position = sample / (float)loaded.frequency;
    }

    private void OnDestroy()
    {
        if (loaded != null) Destroy(loaded);
    }
}
