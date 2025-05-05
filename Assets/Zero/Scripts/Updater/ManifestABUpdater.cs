using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            manifestABPath = FileUtility.CombinePaths(ZeroConst.AB_DIR_NAME, ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION);
            
            if (Runtime.Ins.IsNeedNetwork && false == Runtime.Ins.netResVer.IsSameVer(manifestABPath, Runtime.Ins.localResVer))
            {                
                ILBridge.Ins.StartCoroutine(UpdateManifestAB());
            }
            else
            {
                End();
            }
        }

        IEnumerator UpdateManifestAB()
        {
            var url = FileUtility.CombinePaths(Runtime.Ins.netResDir, manifestABPath);
            Debug.Log(LogColor.Zero2($"[Zero][ManifestABUpdater][{url}] manifest.ab文件更新中..."));
            
            var ver = Runtime.Ins.netResVer.GetVer(manifestABPath);

            var localPath = FileUtility.CombinePaths(Runtime.Ins.localResDir, manifestABPath);

            HttpDownloader loader = new HttpDownloader(url, localPath, ver);
            loader.Start();
            while (false == loader.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            if (null != loader.error)
            {
                Debug.LogError(loader.error);
                End(loader.error);
                yield break;
            }            

            //保存文件版本号
            Runtime.Ins.localResVer.SetVerAndSave(manifestABPath, ver);

            End();
        }
    }
}
