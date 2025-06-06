using Jing;
using System;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 混合的本地版本文件数据模型
    /// 同时加载【热更目录中的ResVerVO(记录热更目录中文件的版本号)】以及【StreamingAssets/res中的ResVerVO(内嵌资源中文件的版本号)】
    /// 混合两个ResVerVO用来作为内存中的版本号数据(以内嵌版本文件为基准，将热更版本号文件覆盖进去)
    /// 写入版本号时，只写入到热更目录中的ResVerVO（因为只有热更文件会更新版本号）
    /// </summary>
    public class LocalMixResVerModel : BaseWriteableResVerModel
    {                
        LocalResVerModel _localResVer;

        public LocalMixResVerModel(ResVerVO streamingAssetsResVer)
        {
            if(null == streamingAssetsResVer)
            {
                throw new Exception("[Zero][LocalMixResVerModel]内嵌资源文件版本号数据不存在，建议直接使用[LocalResVerModel]");
            }

            _vo = streamingAssetsResVer;
            _localResVer = new LocalResVerModel();
            _localResVer.TryCleanCache(_vo);
            Mix();
        }

        public void Mix()
        {
            var list = _localResVer.FindGroup("");
            foreach(var item in list)
            {
                if(item.version != LocalResVerModel.EMPTY_VERSION && GetVer(item.name) != item.version)
                {
                    // Debug.Log(LogColor.Zero2($"[Zero][LocalMixResVerModel] 混合的文件版本号 name:{item.name}  version:{item.version}"));
                    SetVer(item.name, item.version);
                }                
            }
        }

        /// <summary>
        /// 设置文件版本号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public override void SetVerAndSave(string name, string version)
        {            
            _localResVer.SetVerAndSave(name, version);
            SetVer(name, version);
        }

        /// <summary>
        /// 移除指定文件的版本信息
        /// </summary>
        /// <returns>The ver.</returns>
        /// <param name="name">Name.</param>
        public override void RemoveVerAndSave(string name)
        {
            _localResVer.RemoveVerAndSave(name);
            RemoveVer(name);            
        }

        /// <summary>
        /// 清理所有版本信息
        /// </summary>
        public override void ClearVerAndSave()
        {
            _localResVer.ClearVerAndSave();
            ClearVer();            
        }
    }
}
