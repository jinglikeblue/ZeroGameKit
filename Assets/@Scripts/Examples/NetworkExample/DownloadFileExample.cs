using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zero;

namespace Example
{
    /// <summary>
    /// UseCase:
    /// 暂停下载
    /// 继续下载
    /// 销毁下载
    /// </summary>
    class DownloadFileExample
    {
        public static void Start()
        {
            new DownloadFileExample();
        }

        public DownloadFileExample()
        {
            ILBridge.Ins.StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            var url = "http://127.0.0.1/test.zip";
            var savePath = FileUtility.CombinePaths(ZeroConst.PERSISTENT_DATA_PATH, "test.zip");
            var hd = new HttpDownloader(url, savePath, null, true);
            while(false == hd.isDone)
            {
                //var headers = hd.request.GetResponseHeaders();
                //if (headers != null)
                //{
                //    foreach (var kv in headers)
                //    {
                //        Debug.Log($"Header: {kv.Key} = {kv.Value}");
                //    }
                //}
                Debug.LogWarning($"progress:{hd.progress} size:{hd.loadedSize}/{hd.totalSize}");
                yield return null;
            }
            Debug.LogWarning($"下载完成:{hd.progress} error:{hd.error}");
        }



    }
}
