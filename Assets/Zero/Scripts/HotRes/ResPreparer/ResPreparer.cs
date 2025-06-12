using System.Collections.Generic;
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

            List<ResVerVO.Item> needPrepareItemList = new List<ResVerVO.Item>();
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
                            needPrepareItemList.Add(GetItem(path, EResType.Asset));
                        }
                    }
                    
                    if (!WebGL.GetAssetBundle(abName))
                    {
                        needPrepareItemList.Add(GetItem(path, EResType.Asset));
                    }
                }
                else if (resType == EResType.File)
                {
                    if (null == WebGL.GetFile(path))
                    {
                        needPrepareItemList.Add(GetItem(path, EResType.File));
                    }
                }
            }
            
            Info.prepareItems = needPrepareItemList.ToArray();
            Info.totalSize = 0;
            foreach (var item in Info.prepareItems)
            {
                Info.totalSize += item.size;
            }

            #endregion
            


            #region 开始预载

            for (int i = 0; i < Info.prepareItems.Length; i++)
            {
                Info.preparingIndex = i;
                var item = Info.prepareItems[i];
                var loadedSize = Info.loadedSize;
                await WebGL.Prepare(item.name, progress =>
                {
                    Info.unitLoadedSize = (long)(progress * Info.UnitTotalSize);
                    Info.loadedSize = Info.unitLoadedSize + loadedSize;
                    Debug.Log(Info.ToString());
                });
                Info.loadedSize += item.size;
                Info.loadedCount = i + 1;
            }
           

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
        }
    }
}