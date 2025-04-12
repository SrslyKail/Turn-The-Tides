using UnityEngine;
using SFB;
using System.IO;

/// <summary>
/// This class handles opening a file browser and reading in the data to create a new text asset.
/// <para>
/// Relies heavily on <see cref="SFB"/> to run. Credit to them for making such a great library.
/// </para>
/// </summary>
public class LoadExternalJson
{

    private readonly SFB.ExtensionFilter[] extensions = new[]
    {
        new SFB.ExtensionFilter("Data File", new string[]{"json"})
    };

    /// <summary>
    /// Tries to read a json in and create a textasset from it.
    /// <para>
    /// Does not validate the JSON is properly formatted for Turn The Tides.
    /// </para>
    /// </summary>
    /// <param name="textAsset">The returned text asset. Empty if nothing could be read.</param>
    /// <returns>True if the selected file could be read, otherwise False.</returns>
    public bool TryGetDataJson(out TextAsset textAsset)
    {
        textAsset = new TextAsset();
        string[] paths = SelectFile();
        if(paths.Length == 0)
        {
            return false;
        }

        using StreamReader sr = new(paths[0]);
        textAsset = new TextAsset(sr.ReadToEnd());
        return true;
    }

    private string[] SelectFile()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Load Data","", extensions, false);
        return paths;
    }
}
