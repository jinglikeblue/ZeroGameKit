using Jing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestAb : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {        
        var ab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "test_ab.ab"));
        var assets = ab.LoadAllAssets();
        foreach(var asset in assets)
        {
            GameObject.Instantiate(asset);
        }
    }
}
