using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// manifest.ab 文件的更新器
    /// </summary>
    public class ManifestABUpdater : BaseUpdater
    {
        public string manifestABPath { get; private set; }               

        public override void Start()
        {
            base.Start();

            if (WebGL.IsEnvironmentWebGL)
            {
                PrepareManifestForWebGL();
                return;
            }
            
            manifestABPath = FileUtility.CombinePaths(ZeroConst.AB_DIR_NAME, ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION);
            
            if (Runtime.IsNeedNetwork && false == Runtime.netResVer.IsSameVer(manifestABPath, Runtime.localResVer))
            {
                UpdateManifestAB();
            }
            else
            {
                End();
            }
        }

        public async void PrepareManifestForWebGL()
        {
            //预加载manifest.ab;
            await WebGL.PrepareManifestAssetBundle();
            End();
        }

        private async void UpdateManifestAB()
        {
            var url = FileUtility.CombinePaths(Runtime.netResDir, manifestABPath);
            Debug.Log(LogColor.Zero2($"[Zero][ManifestABUpdater][{url}] manifest.ab文件更新中..."));
            
            var ver = Runtime.netResVer.GetVer(manifestABPath);

            var localPath = FileUtility.CombinePaths(Runtime.localResDir, manifestABPath);

            HttpDownloader loader = new HttpDownloader(url, localPath, ver);
            loader.Start();
            while (false == loader.isDone)
            {
                await UniTask.NextFrame();
            }

            if (null != loader.error)
            {
                Debug.LogError(loader.error);
                End(loader.error);
                return;
            }            

            //保存文件版本号
            Runtime.localResVer.SetVerAndSave(manifestABPath, ver);

            End();
        }
    }
}
