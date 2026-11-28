using System;
using System.IO;
using UnityEngine;

[Serializable]
public sealed class AudioPreviewEntry
{
    public int id;
    public string category, branch, name, description, timing, priority, old, revision;
    public string[] sources, projectLayers;
    public AudioPreviewVariant[] variants;
}

[Serializable]
public sealed class AudioPreviewVariant
{
    public string name, mix, miss, prior;
    public float seconds;
    public bool loop;
    public AudioPreviewLayer[] layers;
}

[Serializable]
public sealed class AudioPreviewLayer
{
    public string label, file;
    public float[] marks;
    public bool loop;
    public float seconds;
}

[Serializable]
public sealed class AudioPreviewCatalog
{
    public AudioPreviewEntry[] entries;

    public static AudioPreviewCatalog Load()
    {
        TextAsset data = Resources.Load<TextAsset>("AudioPreview/Catalog");
        return data == null ? null : JsonUtility.FromJson<AudioPreviewCatalog>(data.text);
    }

    public static string Resolve(string reference)
    {
        if (string.IsNullOrEmpty(reference)) return null;
        string root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string demo = Path.Combine(root, "demos", "brotato-audio");
        return Path.GetFullPath(Path.Combine(demo, reference));
    }
}
