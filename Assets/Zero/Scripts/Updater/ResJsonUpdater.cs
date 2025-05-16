using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

            url = FileUtility.CombinePaths(Runtime.netResDir, ZeroConst.RES_JSON_FILE_NAME);
            localPath = FileUtility.CombinePaths(Runtime.localResDir, ZeroConst.RES_JSON_FILE_NAME);

            if (Runtime.IsNeedNetwork)
            {
                UpdateResJson();
            }
            else
            {
                End();
            }
        }

        async void UpdateResJson()
        {
            Debug.Log(LogColor.Zero2($"[Zero][ResJsonUpdater][{url}] res.json文件更新中..."));
            
            var version = DateTime.UtcNow.ToFileTimeUtc().ToString();

            HttpDownloader loader = new HttpDownloader(url, localPath, version);
            loader.Start();

            while (false == loader.isDone)
            {
                await UniTask.NextFrame();
            }

            if (null != loader.error)
            {
                Debug.LogErrorFormat(loader.error);
                End(loader.error);
                return;
            }
            
            End();
        }
    }
}
