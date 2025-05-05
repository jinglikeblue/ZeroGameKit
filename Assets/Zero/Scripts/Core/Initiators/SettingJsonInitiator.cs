using Jing;
using System;
using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// setting.json初始化。加载并根据参数初始化运行时参数。
    /// 如果开启热更，并且勾选了允许离线运行。那么会自动关闭热更功能，继续运行。
    /// </summary>
    internal class SettingJsonInitiator : BaseInitiator
    {
        string _localPath;
        // internal override void Start()
        // {
        //     base.Start();
        //     Debug.Log(LogColor.Zero1("「SettingJsonInitiator」配置文件更新检查..."));
        //     _localPath = FileUtility.CombinePaths(Runtime.Ins.localResDir, "setting.json");
        //
        //     if (Runtime.Ins.IsUseAssetDataBase)
        //     {
        //         SetSetting(new SettingVO());
        //     }
        //     else if(Runtime.Ins.IsNeedNetwork)
        //     {
        //         Update();
        //     }
        //     else if(Runtime.Ins.IsHotResEnable)
        //     {
        //         LoadSettingFromCache();
        //     }
        //     else
        //     {
        //         LoadSettingFromBuiltin();
        //     }
        // }

        internal override async UniTask StartAsync()
        {
            await base.StartAsync();

            Debug.Log(LogColor.Zero1("[Zero]「SettingJsonInitiator」setting.json 文件初始化"));
            _localPath = FileUtility.CombinePaths(Runtime.Ins.localResDir, ZeroConst.SETTING_FILE_NAME);

            if (Runtime.Ins.IsUseAssetDataBase)
            {
                SetSetting(new SettingVO());
                return;
            }

            //热更模式下，先尝试更新setting.json
            if (Runtime.Ins.IsHotResEnable)
            {
                var isUpdateSuccess = await Update();
                if (false == isUpdateSuccess)
                {
                    if (false == Runtime.Ins.IsOfflineEnable)
                    {
                        throw new Exception($"[Zero][SettingJsonInitiator] setting.json更新失败！请检查网络连接！");    
                    }
                    else
                    {
                        //关闭热更功能
                        Runtime.Ins.VO.isHotPatchEnable = false;
                    }
                }
            }

            var settingJsonString = await HotRes.LoadString(ZeroConst.SETTING_FILE_NAME);
            SetSetting(Json.ToObject<SettingVO>(settingJsonString));
        }

        void SetSetting(SettingVO vo)
        {
            if (vo.lsLogEnable.isOverride)
            {
                Runtime.Ins.SetLogEnable(vo.lsLogEnable.value);
            }

            if (vo.lsUseDll.isOverride)
            {
                Runtime.Ins.VO.isUseDll = vo.lsUseDll.value;
            }

            Runtime.Ins.setting = vo;

            //如果配置了资源地址重定向，则更改网络资源路径
            if (false == string.IsNullOrEmpty(vo.netResRoot))
            {
                Runtime.Ins.netResDir = FileUtility.CombineDirs(true, vo.netResRoot, ZeroConst.PLATFORM_DIR_NAME);
            }

            End();
        }

        async UniTask<bool> Update()
        {
            var list = Runtime.Ins.SettingFileNetDirList;
            for (var i = 0; i < list.Length; i++)
            {
                var settingFileUrl = FileUtility.CombinePaths(list[i], ZeroConst.SETTING_FILE_NAME);
                Debug.Log(LogColor.Zero1($"[Zero][SettingJsonInitiator] 开始下载setting.json: {settingFileUrl}"));
                HttpDownloader loader = new HttpDownloader(settingFileUrl, _localPath, DateTime.UtcNow.ToFileTimeUtc().ToString());
                loader.Start();
                while (false == loader.isDone)
                {
                    await UniTask.NextFrame();
                }

                if (null == loader.error)
                {
                    //设置网络资源地址
                    Runtime.Ins.netResDir = list[i];
                    return true;
                }

                Debug.Log(LogColor.Zero2($"[Zero][SettingJsonInitiator] setting.json下载失败({loader.error}): {settingFileUrl}"));
            }

            var errorMsg = "所有指向setting.json的URL都不可使用！";
            Debug.LogError(errorMsg);
            End(errorMsg);
            return false;
        }
    }
}