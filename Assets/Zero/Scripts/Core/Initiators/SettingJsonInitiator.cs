using Jing;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Zero
{
    internal class SettingJsonInitiator : BaseInitiator
    {
        string _localPath;
        internal override void Start()
        {
            base.Start();
            Debug.Log(Log.Zero1("「SettingJsonInitiator」配置文件更新检查..."));
            _localPath = FileUtility.CombinePaths(Runtime.Ins.localResDir, "setting.json");



            if(Runtime.Ins.localData.IsUpdateSetting && Runtime.Ins.IsNeedNetwork)
            {
                ILBridge.Ins.StartCoroutine(Update());
            }
            else if(Runtime.Ins.BuiltinResMode != EBuiltinResMode.ONLY_USE)
            {
                LoadSettingFromCache();
            }
            else
            {
                LoadSettingFromBuiltin();
            }
        }

        void LoadSettingFromCache()
        {
            string settingJsonStr = File.ReadAllText(_localPath);
            SettingVO vo = LitJson.JsonMapper.ToObject<SettingVO>(settingJsonStr);
            SetSetting(vo);
        }

        void LoadSettingFromBuiltin()
        {
            SetSetting(Runtime.Ins.streamingAssetsResInitiator.settingVO);
        }

        void SetSetting(SettingVO vo)
        {
            //TODO 这里需要用setting.json里配置的数据，覆盖preload中配置的RuntimeVO数据
            //Runtime.Ins.VO

            Runtime.Ins.setting = vo;
            if (vo.netResRoot != null)
            {
                Runtime.Ins.netResDir = FileUtility.CombineDirs(true, vo.netResRoot, ZeroConst.PLATFORM_DIR_NAME);
            }
            End();
        }

        IEnumerator Update()
        {
            var list = Runtime.Ins.SettingFileNetDirList;
            for (var i = 0; i < list.Length; i++)
            {
                var settingFileUrl = FileUtility.CombinePaths(list[i], "setting.json");
                Debug.Log(Log.Zero1("开始下载setting.json: {0}", settingFileUrl));
                Downloader loader = new Downloader(settingFileUrl, _localPath, DateTime.UtcNow.ToFileTimeUtc().ToString());
                while (false == loader.isDone)
                {
                    yield return new WaitForEndOfFrame();
                }
                loader.Dispose();
                if (null == loader.error)
                {
                    LoadSettingFromCache();
                    yield break;
                }
                else
                {
                    Debug.Log(Log.Zero2($"setting.json下载失败({loader.error}): {settingFileUrl}"));
                }
            }

            var errorMsg = "所有指向setting.json的URL都不可使用！";
            Debug.LogErrorFormat(errorMsg);
            End(errorMsg);            
        }
    }
}
