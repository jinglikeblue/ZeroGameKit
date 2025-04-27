using Jing;
using System.IO;

namespace Zero
{
    internal class StartupResInitiator : BaseInitiator
    {
        ResJsonUpdater _resJsonUpdater;

        ManifestABUpdater _manifestABUpdater;

        HotResUpdater _hotResUpdater;

        internal override void Start()
        {
            base.Start();
            if (Runtime.Ins.IsNeedNetwork)
            {                
                UpdateResJson();
            }
            else
            {
                InitResMgr();
                End();
            }
        }

        void UpdateResJson()
        {
            _resJsonUpdater = new ResJsonUpdater();
            _resJsonUpdater.onComplete += OnResJsonUpdaterComplete;
            _resJsonUpdater.Start();
        }

        private void OnResJsonUpdaterComplete(BaseUpdater updater)
        {
            updater.onComplete -= OnResJsonUpdaterComplete;
            if(updater.error != null)
            {
                End(updater.error);
            }
            else
            {                
                string jsonStr = File.ReadAllText(_resJsonUpdater.localPath);
                ResVerVO vo = Json.ToObject<ResVerVO>(jsonStr);
                Runtime.Ins.netResVer = new ResVerModel(vo);
                UpdateManifestAB();
            }
        }

        void UpdateManifestAB()
        {
            _manifestABUpdater = new ManifestABUpdater();
            _manifestABUpdater.onComplete += OnManifestABUpdaterComplete;
            _manifestABUpdater.Start();
        }

        void OnManifestABUpdaterComplete(BaseUpdater updater)
        {
            updater.onComplete -= OnResJsonUpdaterComplete;
            if (updater.error != null)
            {
                End(updater.error);
            }
            else
            {
                InitResMgr();
                UpdateStartupRes();
            }
        }

        void InitResMgr()
        {
            //因为更新了manifest.ab文件，所以要重新初始化ResMgr的Init
            if (Runtime.Ins.IsLoadAssetBundleByAssetDataBase)
            {
                ResMgr.Init(ResMgr.EResMgrType.AssetDataBase, ZeroConst.HOT_RESOURCES_ROOT_DIR);
            }
            else
            {
                var manifestFileName = ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION;
                ResMgr.Init(ResMgr.EResMgrType.AssetBundle, manifestFileName);
            }
        }

        /// <summary>
        /// 更新启动资源
        /// </summary>
        void UpdateStartupRes()
        {            
            _hotResUpdater = new HotResUpdater(Runtime.Ins.setting.startupResGroups);
            _hotResUpdater.onProgress += OnHotResUpdaterProgress;
            _hotResUpdater.onComplete += OnHotResUpdaterComplete;
            _hotResUpdater.Start();
        }

        private void OnHotResUpdaterComplete(BaseUpdater updater)
        {
            _hotResUpdater.onProgress -= OnHotResUpdaterProgress;
            _hotResUpdater.onComplete -= OnHotResUpdaterComplete;

            if (updater.error != null)
            {
                End(updater.error);
            }
            else
            {
                End();
            }            
        }

        private void OnHotResUpdaterProgress(long loadedSize, long totalSize)
        {
            Progress(loadedSize, totalSize);
        }
    }
}
