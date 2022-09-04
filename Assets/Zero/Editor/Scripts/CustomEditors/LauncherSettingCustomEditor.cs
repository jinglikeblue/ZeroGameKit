using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    [CustomEditor(typeof(LauncherSetting))]
    public class LauncherSettingCustomEditor : OdinEditor
    {       
        LauncherSetting Target
        {
            get
            {
                return this.target as LauncherSetting;
            }            
        }

        override protected void OnEnable()
        {
            Target.data = Load();
            Target.data.onChange += OnSettingChanged;
            Target.data.onILTypeChanged += OnILTypeChanged;
        }

        private void OnSettingChanged()
        {
            Save(Target.data);
        }

        override protected void OnDisable()
        {
            Target.data.onChange -= OnSettingChanged;
            Target.data.onILTypeChanged += OnILTypeChanged;            
        }

        private void OnILTypeChanged()
        {
            //同步HybridCLR环境
            HybridCLRUtility.SyncWithHybridCLRSettings();
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

            Debug.Log($"读取[LauncherSettingData]");

            _cache = LauncherSetting.LoadLauncherSettingDataFromResources();
            if (null == _cache)
            {
                _cache = new LauncherSettingData();
            }

            return _cache;
        }


        static public void Save(LauncherSettingData vo)
        {
            if (vo == null)
            {
                throw new Exception("保存的[LauncherSettingData]为null!!!");
            }

            _cache = vo;

            Debug.Log($"保存[LauncherSettingData]");
            var jsonStr = LitJson.JsonMapper.ToPrettyJson(_cache);
            File.WriteAllText($"Assets/Resources/{ZeroConst.LAUNCHER_SETTING_NAME}.json", jsonStr);
            AssetDatabase.Refresh();

            onValueChanged?.Invoke();
        }    
    }
}