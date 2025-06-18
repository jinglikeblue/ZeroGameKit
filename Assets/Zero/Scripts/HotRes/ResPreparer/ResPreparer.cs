using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源准备器
    /// </summary>
    public class ResPreparer
    {
        public ResPrepareProgressInfoVO Info { get; private set; }

        public bool IsDone { get; private set; } = false;

        public string[] Paths { get; private set; }

        public ResPreparer(string[] paths)
        {
            Info = new ResPrepareProgressInfoVO();
            Paths = paths;
        }

        public async void Start()
        {
            do
            {
                if (Runtime.IsUseAssetBundle)
                {
                    if (WebGL.IsEnvironmentWebGL)
                    {
                        await StartPrepareForWebGL();
                        break;
                    }

                    if (Runtime.IsHotResEnable)
                    {
                        await StartPrepareHotRes();
                        break;
                    }
                }
            } while (false);

            IsDone = true;
        }

        private async UniTask StartPrepareForWebGL()
        {
            #region 计算要预载的资源

            HashSet<ResVerVO.Item> needPrepareItemSet = new HashSet<ResVerVO.Item>();
            //检查path中，哪些已有缓存，过滤掉不需要预载的资源。
            foreach (var path in Paths)
            {
                var resType = Res.GetResType(path);
                if (resType == EResType.Asset)
                {
                    var abName = Res.TransformToHotPath(path, EResType.Asset);
                    var dependsList = Assets.GetDepends(abName);
                    foreach (var dependPath in dependsList)
                    {
                        var dependABName = Res.TransformToHotPath(dependPath, EResType.Asset);
                        if (!WebGL.GetAssetBundle(dependABName))
                        {
                            needPrepareItemSet.Add(GetItem(path, EResType.Asset));
                        }
                    }
                    
                    if (!WebGL.GetAssetBundle(abName))
                    {
                        needPrepareItemSet.Add(GetItem(path, EResType.Asset));
                    }
                }
                else if (resType == EResType.File)
                {
                    if (null == WebGL.GetFile(path))
                    {
                        needPrepareItemSet.Add(GetItem(path, EResType.File));
                    }
                }
            }
            
            Info.prepareItems = needPrepareItemSet.ToArray();
            Info.totalSize = 0;
            foreach (var item in Info.prepareItems)
            {
                Info.totalSize += item.size;
            }

            #endregion

            #region 开始预载

            long loadedSize = 0;
            for (int i = 0; i < Info.prepareItems.Length; i++)
            {
                Info.preparingIndex = i;
                var item = Info.prepareItems[i];
                await WebGL.Prepare(item.name, progress =>
                {
                    Info.unitLoadedSize = (long)(progress * Info.UnitTotalSize);
                    Info.loadedSize = Info.unitLoadedSize + loadedSize;
                    Debug.Log(Info.ToString());
                });
                loadedSize += item.size;
                Info.loadedSize = loadedSize;
                Info.loadedCount = i + 1;
            }
            Debug.Log(Info.ToString());

            #endregion

            ResVerVO.Item GetItem(string path, EResType resType)
            {
                var itemName = Res.TransformToHotPath(path, resType);
                var item = Runtime.localResVer.Get(itemName);
                return item;
            }
        }

        private async UniTask StartPrepareHotRes()
        {
            //TODO 考虑是否需要实现
            return;
        }
    }
}