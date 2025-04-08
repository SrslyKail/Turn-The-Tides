using UnityEngine;
using SFB;
using System.Windows.Forms.VisualStyles;
using System.Threading;
using System.IO;

public class LoadExternalJson: MonoBehaviour
{

    private readonly SFB.ExtensionFilter[] extensions = new[]
    {
        new SFB.ExtensionFilter("Data File", new string[]{"json"})
    };

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

    private void Awake()
    {
        SelectFile();
    }

    private string[] SelectFile()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Load Data","", extensions, false);
        return paths;
    }
}
