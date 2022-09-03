using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zero;

public class HybridCLRSettings : ScriptableObject
{
    const string assetPath = "Assets/Zero/HybridCLR/hybridclr_settings.asset";

    static HybridCLRSettings _ins;

    static public HybridCLRSettings Ins
    {
        get
        {
            if(null == _ins)
            {
#if UNITY_EDITOR
                _ins = AssetDatabase.LoadAssetAtPath<HybridCLRSettings>(assetPath);

                if (_ins == null)
                {
                    _ins = ScriptableObject.CreateInstance<HybridCLRSettings>();

                    AssetDatabase.CreateAsset(_ins, assetPath);
                    AssetDatabase.SaveAssets();
                }
#endif
            }

            return _ins;
        }
    }

    /// <summary>
    /// Preload的DLL执行方式改变
    /// </summary>
    public event Action<HybridCLRSettings> onEnableChanged;

    /// <summary>
    /// 是否使用HybridCLR
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [SuffixLabel("HybridCLR模式是否开启")]
    bool _isHybridCLREnable = false;

    public bool IsHybridCLREnable
    {
        get
        {
            return _isHybridCLREnable;
        }

        set
        {
            _isHybridCLREnable = value;
            onEnableChanged?.Invoke(this);            
        }
    }
}
