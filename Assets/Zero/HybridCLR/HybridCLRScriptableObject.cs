using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HybridCLRScriptableObject : ScriptableObject
{
    const string assetPath = "Assets/Zero/HybridCLR/settings.asset";

    static public HybridCLRScriptableObject Ins
    {
        get
        {
            var settings = AssetDatabase.LoadAssetAtPath<HybridCLRScriptableObject>(assetPath);

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<HybridCLRScriptableObject>();
                
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();                
            }

            return settings;
        }
    }
    /// <summary>
    ///  «∑Ò π”√HybridCLR
    /// </summary>
    [SerializeField]
    public bool isHybridCLREnable = false;
}
