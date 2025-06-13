using System;
using System.Collections.Generic;
using Jing;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    internal class StartupResInitiator : BaseInitiator
    {
        ResJsonUpdater _resJsonUpdater;

        ManifestABUpdater _manifestABUpdater;

        HotResUpdater _hotResUpdater;

        private InitiatorProgress _onProgress;

        internal override async UniTask<string> StartAsync(InitiatorProgress onProgress)
        {
            _onProgress = onProgress;

            string err = null;
            do
            {
                if (Runtime.IsHotResEnable)
                {
                    //更新res.json
                    err = await new ResJsonUpdater().StartAsync();
                    if (!string.IsNullOrEmpty(err)) break;

                    string jsonStr = await Res.LoadAsync<string>(ZeroConst.RES_JSON_FILE_NAME);
                    ResVerVO vo = Json.ToObject<ResVerVO>(jsonStr);
                    Runtime.netResVer = new NetResVerModel(vo);
                    Runtime.netResVer.TryCleanCache();
                }

                if (Runtime.IsHotResEnable || WebGL.IsEnvironmentWebGL)
                {
                    //更新manifest.ab
                    err = await new ManifestABUpdater().StartAsync();
                    if (!string.IsNullOrEmpty(err)) break;
                }
                
                if (WebGL.IsEnvironmentWebGL)
                {
                    err = await PrepareStartupResForWebGL();
                    break;
                }

                // 初始化ResMgr，依赖manifest.ab
                // InitResMgr();

                if (Runtime.IsHotResEnable)
                {
                    //检查启动资源更新。依赖ResMgr
                    err = await new HotResUpdater(GetStartupResGroups()).StartAsync(OnHotResUpdaterProgress);
                    if (!string.IsNullOrEmpty(err)) break;
                }
            } while (false);

            return err;
        }

        private void OnHotResUpdaterProgress(long loadedSize, long totalSize)
        {
            _onProgress(loadedSize, totalSize);
        }

        string[] GetStartupResGroups()
        {
            if (null == Runtime.setting.startupResGroups || 0 == Runtime.setting.startupResGroups.Length)
            {
                //未配置启动必要资源组，默认下载所有的资源
                Runtime.setting.startupResGroups = new[] { "/" };
            }

            return Runtime.setting.startupResGroups;
        }

        private async UniTask<string> PrepareStartupResForWebGL()
        {
            try
            {
                //组织需要的启动资源
                var groups = GetStartupResGroups();
                // var itemNames = new HashSet<string>();
                // foreach (var group in groups)
                // {
                //     var itemList = Runtime.localResVer.FindGroup(group);
                //     foreach (var item in itemList)
                //     {
                //         itemNames.Add(item.name);
                //     }
                // }

                var itemNames = Res.GetGroupResArray(groups);
                await Res.Prepare(itemNames.ToArray(), info => { OnHotResUpdaterProgress(info.loadedSize, info.totalSize); });
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return e.ToString();
            }
        }
    }
}