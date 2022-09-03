using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zero;

[ExecuteInEditMode]
public class LauncherSetter : MonoBehaviour
{
    public RuntimeVO runtimeVO;

#if UNITY_EDITOR

    private void Awake()
    {
        runtimeVO = Load();
    }

    private void OnEnable()
    {        
        runtimeVO.onChange += OnSettingChanged;
    }

    private void OnSettingChanged()
    {
        Save(runtimeVO);        
    }

    private void OnDisable()
    {        
        runtimeVO.onChange -= OnSettingChanged;
        Save(runtimeVO);
    }

    #region Static ¾²Ì¬´úÂë

    static RuntimeVO _cache;

    static public event Action onValueChanged;

    static public RuntimeVO Load()
    {
        if(_cache != null)
        {
            return _cache;
        }

        Debug.Log($"LauncherSetter:Load");

        var ta = Resources.Load<TextAsset>("launcher_setting");
        if (null == ta)
        {
            _cache = new RuntimeVO();
        }
        else
        {
            _cache = LitJson.JsonMapper.ToObject<RuntimeVO>(ta.text);
        }
        
        return _cache;
    }

    static public void Save(RuntimeVO vo)
    {    
        if(vo == null)
        {
            throw new Exception("fuck you!");
        }

        _cache = vo;

        Debug.Log($"LauncherSetter:Save");
        var jsonStr = LitJson.JsonMapper.ToPrettyJson(_cache);
        File.WriteAllText("Assets/Resources/launcher_setting.json", jsonStr);
        AssetDatabase.Refresh();

        onValueChanged?.Invoke();
    }

    #endregion
#endif
}
