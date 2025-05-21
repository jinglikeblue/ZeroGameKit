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
    /// 热更资源更新器。该更新器需要res.json和manifest.ab已初始化。
    /// </summary>
    public class HotResUpdater : BaseUpdater
    {
        public string[] CheckNameList { get; private set; }

        public string[] UpdateResNameList { get; private set; }

        /// <summary>
        /// Start()调用前设置该属性有效。是否忽略版本检查。强制更新资源。
        /// 注意：推荐设置为false，因为框架会记录下载资源的版本号，如果本地已有对应版本资源，不会重复下载
        /// </summary>
        public bool IsForceUpdateAll = false;

        public HotResUpdater(string name)
        {
            CheckNameList = new string[] { name };
        }

        public HotResUpdater(string[] groups)
        {
            CheckNameList = groups;
        }

        public override void Start()
        {
            base.Start();
            if (Runtime.IsNeedNetwork)
            {
                //检查要更新的资源列表
                UpdateResNameList = CheckNeedUpdateResNameList(CheckNameList, IsForceUpdateAll);
                UpdateRes();
            }
            else
            {
                UpdateResNameList = Array.Empty<string>();
                End();
            }
        }

        private async void UpdateRes()
        {
            //实例化一个资源组下载器
            GroupHttpDownloader groupLoader = new GroupHttpDownloader();
            foreach (var resName in UpdateResNameList)
            {
                var netItem = Runtime.netResVer.Get(resName);

                //将要下载的文件依次添加入下载器
                groupLoader.AddTask(FileUtility.CombinePaths(Runtime.netResDir, resName), FileUtility.CombinePaths(Runtime.localResDir, resName), netItem.version, netItem.size, netItem);
            }

            groupLoader.onTaskCompleted += OnGroupHttpDownloaderTaskCompleted;

            //启动下载器开始下载
            groupLoader.Start();

            //判断是否所有资源下载完成，如果没有，返回一个下载的进度（该进度表示的整体进度）
            do
            {
                Progress(groupLoader.loadedSize, groupLoader.totalSize);
                await UniTask.NextFrame();
                if (CancelToken.IsCancellationRequested)
                {
                    groupLoader.StopAndDispose(true);
                }
            }
            while (false == groupLoader.isDone);

            groupLoader.onTaskCompleted -= OnGroupHttpDownloaderTaskCompleted;

            //判断下载是否返回错误
            if (null != groupLoader.error)
            {
                End(groupLoader.error);
                return;
            }

            End();
        }

        // IEnumerator UpdateGroups()
        // {
        //     //实例化一个资源组下载器
        //     GroupHttpDownloader groupLoader = new GroupHttpDownloader();
        //     foreach (var resName in UpdateResNameList)
        //     {
        //         var netItem = Runtime.netResVer.Get(resName);
        //
        //         //将要下载的文件依次添加入下载器
        //         groupLoader.AddTask(FileUtility.CombinePaths(Runtime.netResDir, resName), FileUtility.CombinePaths(Runtime.localResDir, resName), netItem.version, netItem.size, netItem);
        //     }
        //
        //     groupLoader.onTaskCompleted += OnGroupHttpDownloaderTaskCompleted;
        //
        //     //启动下载器开始下载
        //     groupLoader.Start();
        //
        //     //判断是否所有资源下载完成，如果没有，返回一个下载的进度（该进度表示的整体进度）
        //     do
        //     {
        //         Progress(groupLoader.loadedSize, groupLoader.totalSize);                
        //         yield return new WaitForEndOfFrame();
        //     }
        //     while (false == groupLoader.isDone);
        //
        //     groupLoader.onTaskCompleted -= OnGroupHttpDownloaderTaskCompleted;
        //
        //     //判断下载是否返回错误
        //     if (null != groupLoader.error)
        //     {
        //         End(groupLoader.error);
        //         yield break;
        //     }
        //
        //     End();
        // }

        private void OnGroupHttpDownloaderTaskCompleted(GroupHttpDownloader groupDownloader, GroupHttpDownloader.TaskInfo taskInfo)
        {
            var item = (ResVerVO.Item)taskInfo.data;
            double size =  Math.Round((double)groupDownloader.currentDownloader.totalSize / 1024 / 1024, 2);
            Debug.Log(LogColor.Zero1($"下载完成:{item.name} Size:{size}MB Ver:{item.version}"));
            Runtime.localResVer.SetVerAndSave(item.name, item.version);
        }

        #region 检查要更新的资源列表

        public static string[] CheckNeedUpdateResNameList(string name)
        {
            return CheckNeedUpdateResNameList(new string[] { name });
        }

        /// <summary>
        /// 检查目标资源组对应的更新列表
        /// <para>注意：该方法并不会更新res.json以及ab.mainifest文件</para>
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="isForceUpdateAll">false(默认值):返回框架判断需要更新的资源，true:返回所有资源</param>
        /// <returns></returns>
        public static string[] CheckNeedUpdateResNameList(string[] groups, bool isForceUpdateAll = false)
        {
            //整理出所有需要资源的清单（包括依赖的）
            HashSet<string> itemSet = new HashSet<string>();
            for (int i = 0; i < groups.Length; i++)
            {
                string group = groups[i];
                var itemList = GetItemsInGroup(group);
                foreach (var itemName in itemList)
                {
                    itemSet.Add(itemName);
                }
            }

            List<string> needUpdateList = new List<string>();
            //开始检查版本，找出需要更新的资源
            foreach (var itemName in itemSet)
            {
                if (isForceUpdateAll)
                {
                    needUpdateList.Add(itemName);
                    continue;
                }
                
                string localVer = Runtime.localResVer.GetVer(itemName);
                var netItem = Runtime.netResVer.Get(itemName);

                if (localVer != netItem.version)
                {
                    //版本一致，不需要更新
                    needUpdateList.Add(itemName);
                }
            }

            return needUpdateList.ToArray();
        }

        static List<string> GetItemsInGroup(string group)
        {
            List<string> nameList = new List<string>();
            var itemList = Runtime.netResVer.FindGroup(group);

            foreach (var item in itemList)
            {
                nameList.Add(item.name);
                var depends = GetAllDepends(item.name);
                nameList.AddRange(depends);

                // var logSB = new StringBuilder();
                // logSB.AppendLine(LogColor.Zero2("进行版本校验的资源：{0}", item.name));
                // if (depends.Count > 0)
                // {
                //     logSB.AppendLine(LogColor.Zero2("                 依赖的资源:"));
                //     foreach (var depend in depends)
                //     {
                //         logSB.AppendLine(LogColor.Zero2("                 {0}", depend));
                //     }
                // }
                // if (logSB.Length > 0)
                // {
                //     Debug.Log(logSB.ToString());
                // }
            }

            return nameList;
        }

        /// <summary>
        /// 得到目标所有的依赖
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        static List<string> GetAllDepends(string itemName)
        {
            List<string> nameList = new List<string>();
            string abDir = Zero.ZeroConst.AB_DIR_NAME;
            abDir += "/";
            if (false == itemName.StartsWith(abDir))
            {
                return nameList;
            }


            string abName = itemName.Replace(abDir, "");
            var abDependList = Assets.GetDepends(abName);
            foreach (var ab in abDependList)
            {
                nameList.Add(FileUtility.CombinePaths(abDir, ab));
            }

            return nameList;
        }

        #endregion
    }
}
