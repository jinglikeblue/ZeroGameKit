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
    /// Preload��DLLִ�з�ʽ�ı�
    /// </summary>
    public event Action<HybridCLRSettings> onEnableChanged;

    /// <summary>
    /// �Ƿ�ʹ��HybridCLR
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [SuffixLabel("HybridCLRģʽ�Ƿ���")]
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
