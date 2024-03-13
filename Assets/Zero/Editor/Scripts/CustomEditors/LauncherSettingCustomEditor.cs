using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        private void OnSceneSaved(Scene scene)
        {
            Save(Target.data);
        }

        private void OnSettingChanged()
        {
            _isDirty = true;
            //Save(Target.data);
        }

        override protected void OnDisable()
        {
            Target.data.onChange -= OnSettingChanged;
            Target.data.onILTypeChanged -= OnILTypeChanged;
            EditorSceneManager.sceneSaved -= OnSceneSaved;
            Save(Target.data);
        }

        private void OnILTypeChanged()
        {
            //同步HybridCLR环境
            HybridCLRUtility.SyncWithHybridCLRSettings();
        }

        static public event Action onValueChanged;

        static LauncherSettingData _cache;

        static bool _isDirty = false;

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
                _isDirty = true;
            }

            return _cache;
        }


        static public void Save(LauncherSettingData vo, bool isOnlySaveIfDirty = true)
        {
            if (isOnlySaveIfDirty)
            {
                if(false == _isDirty)
                {
                    return;
                }
                _isDirty = false;
            }

            if (vo == null)
            {
                throw new Exception("保存的[LauncherSettingData]为null!!!");
            }

            _cache = vo;

            Debug.Log($"保存[LauncherSettingData]");
            var jsonStr = Json.ToJsonIndented(_cache);
            File.WriteAllText($"Assets/Resources/{ZeroConst.LAUNCHER_SETTING_NAME}.json", jsonStr);
            AssetDatabase.Refresh();

            onValueChanged?.Invoke();
        }    
    }
}