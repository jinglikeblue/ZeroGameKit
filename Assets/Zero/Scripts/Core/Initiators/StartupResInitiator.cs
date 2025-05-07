using Jing;
using System.IO;
using Cysharp.Threading.Tasks;

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
                if (Runtime.Ins.IsHotResEnable)
                {
                    //更新res.json
                    err = await new ResJsonUpdater().StartAsync();
                    if (!string.IsNullOrEmpty(err)) break;

                    string jsonStr = await HotRes.LoadString(ZeroConst.RES_JSON_FILE_NAME);
                    ResVerVO vo = Json.ToObject<ResVerVO>(jsonStr);
                    Runtime.Ins.netResVer = new ResVerModel(vo);

                    //更新manifest.ab
                    err = await new ManifestABUpdater().StartAsync();
                    if (!string.IsNullOrEmpty(err)) break;
                }

                // 初始化ResMgr，依赖manifest.ab
                InitResMgr();

                if (Runtime.Ins.IsHotResEnable)
                {
                    if (null == Runtime.Ins.setting.startupResGroups || 0 == Runtime.Ins.setting.startupResGroups.Length)
                    {
                        //未配置启动必要资源组，默认下载所有的资源
                        Runtime.Ins.setting.startupResGroups = new[] { "/" };
                    }
                    
                    //检查启动资源更新。依赖ResMgr
                    err = await new HotResUpdater(Runtime.Ins.setting.startupResGroups).StartAsync(OnHotResUpdaterProgress);
                    if (!string.IsNullOrEmpty(err)) break;
                }
            } while (false);

            return err;
        }

        void InitResMgr()
        {
            //因为更新了manifest.ab文件，所以要重新初始化ResMgr的Init
            if (Runtime.Ins.IsUseAssetDataBase)
            {
                ResMgr.Init(ResMgr.EResMgrType.AssetDataBase, ZeroConst.HOT_RESOURCES_ROOT_DIR);
            }
            else
            {
                var manifestFileName = ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION;
                ResMgr.Init(ResMgr.EResMgrType.AssetBundle, manifestFileName);
            }
        }

        private void OnHotResUpdaterProgress(long loadedSize, long totalSize)
        {
            _onProgress(loadedSize, totalSize);
        }
    }
}