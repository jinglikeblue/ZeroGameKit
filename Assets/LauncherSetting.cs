using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zero;

[ExecuteInEditMode]
public class LauncherSetting : MonoBehaviour
{
    public LauncherSettingData runtimeVO;

    static LauncherSettingData _cache;

    static public LauncherSettingData Load()
    {
        if (_cache != null)
        {
            return _cache;
        }

        Debug.Log($"LauncherSetter:Load");

        var ta = Resources.Load<TextAsset>("launcher_setting");
        if (null == ta)
        {
            _cache = new LauncherSettingData();
        }
        else
        {
            _cache = LitJson.JsonMapper.ToObject<LauncherSettingData>(ta.text);
        }

        return _cache;
    }

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

    static public event Action onValueChanged;

    static public void Save(LauncherSettingData vo)
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
#endif
}
