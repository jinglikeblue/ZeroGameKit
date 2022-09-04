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

    /// <summary>
    /// 加载Resources中的launcher_setting_data，并返回全新的对象
    /// </summary>
    /// <returns></returns>
    static public LauncherSettingData LoadLauncherSettingDataFromResources()
    {
        var ta = Resources.Load<TextAsset>("launcher_setting");
        if(null != ta)
        {
            return LitJson.JsonMapper.ToObject<LauncherSettingData>(ta.text);
        }
        return null;
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

    static LauncherSettingData _cache;

    /// <summary>
    /// 加载LauncherSettingData，如果有缓存则返回，无则从Resources中加载
    /// </summary>
    /// <returns></returns>
    static public LauncherSettingData Load()
    {
        if (_cache != null)
        {
            return _cache;
        }

        Debug.Log($"LauncherSetter:Load");

        _cache = LoadLauncherSettingDataFromResources();
        if (null == _cache)
        {
            _cache = new LauncherSettingData();
        }

        return _cache;
    }


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
