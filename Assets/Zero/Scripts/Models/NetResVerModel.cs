using System.IO;
using Jing;
using UnityEngine;

namespace Zero
{
    public class NetResVerModel : ResVerModel
    {
        public NetResVerModel(ResVerVO vo) : base(vo)
        {
        }

        /// <summary>
        /// 尝试清理缓存
        /// </summary>
        public void TryCleanCache()
        {
            if (!Runtime.IsHotResEnable)
            {
                //没有使用资源的情况，不处理
                return;
            }

            //从caches里删除掉网络res.json中没有记录的文件。优化空间
            foreach (var item in Runtime.localResVer.VO.items)
            {
                if (null == GetVer(item.name))
                {
                    var path = FileUtility.CombinePaths(ZeroConst.WWW_RES_PERSISTENT_DATA_PATH, item.name);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        Debug.Log($"[Zero][Cache][网络资源更新] 删除无用的缓存文件: {path}");
                    }
                }
            }
        }
    }
}