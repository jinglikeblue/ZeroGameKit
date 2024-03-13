using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// res.json文件更新器
    /// </summary>
    public class ResJsonUpdater : BaseUpdater
    {
        public string url { get; private set; }
        public string localPath { get; private set; }

        public override void Start()
        {
            base.Start();

            Debug.Log(LogColor.Zero1("「ResJsonUpdater」res.json文件更新检查..."));

            url = FileUtility.CombinePaths(Runtime.Ins.netResDir, ZeroConst.RES_JSON_FILE_NAME);
            localPath = FileUtility.CombinePaths(Runtime.Ins.localResDir, ZeroConst.RES_JSON_FILE_NAME);

            if (Runtime.Ins.IsNeedNetwork)
            {
                ILBridge.Ins.StartCoroutine(UpdateResJson());
            }
            else
            {
                End();
            }
        }

        IEnumerator UpdateResJson()
        {
            Debug.Log(LogColor.Zero1($"「ResJsonUpdater」res.json文件更新中...  [{url}]"));
            
            var version = DateTime.UtcNow.ToFileTimeUtc().ToString();

            HttpDownloader loader = new HttpDownloader(url, localPath, version);
            loader.Start();

            while (false == loader.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            if (null != loader.error)
            {
                Debug.LogErrorFormat(loader.error);
                End(loader.error);
                yield break;
            }
            
            End();
        }
    }
}
