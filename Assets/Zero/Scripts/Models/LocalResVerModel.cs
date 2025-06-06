using System;
using Jing;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 本地版本文件数据模型
    /// </summary>
    public class LocalResVerModel : BaseWriteableResVerModel
    {
        const string FILE_NAME = "local_res_ver.zero.json";
        string _path;

        public const string EMPTY_VERSION = "";

        public LocalResVerModel()
        {
            Load();
        }

        public void Load()
        {
            _path = FileUtility.CombinePaths(Runtime.generateFilesDir, FILE_NAME);

            if (File.Exists(_path))
            {
                //读取已有的数据
                _vo = Json.ToObject<ResVerVO>(File.ReadAllText(_path));
            }
            else
            {
                //新数据初始化
                _vo = new ResVerVO();
            }

            if (_vo.items == null)
            {
                _vo.items = Array.Empty<ResVerVO.Item>();
            }
            else
            {
                ConformingLocalRes();
            }
        }

        /// <summary>
        /// 检查本地的文件，是否存在或者版本号是否一致
        /// <para>PS：版本号一致需要MD5验证，开销较大，暂不加入</para>
        /// </summary>
        public void ConformingLocalRes()
        {
            foreach (var item in _vo.items)
            {
                var filePath = FileUtility.CombinePaths(Runtime.localResDir, item.name);
                if (!File.Exists(filePath))
                {
                    SetVer(item.name, EMPTY_VERSION);
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
            SetVer(name, version);
            Save();
        }

        /// <summary>
        /// 移除指定文件的版本信息
        /// </summary>
        /// <returns>The ver.</returns>
        /// <param name="name">Name.</param>
        public override void RemoveVerAndSave(string name)
        {
            RemoveVer(name);
            Save();
        }

        /// <summary>
        /// 清理所有版本信息
        /// </summary>
        public override void ClearVerAndSave()
        {
            ClearVer();
            Save();
        }

        public void Save()
        {
            string json = Json.ToJsonIndented(_vo);
            File.WriteAllText(_path, json);
        }

        /// <summary>
        /// 尝试清理缓存
        /// </summary>
        public void TryCleanCache(ResVerVO builtinResVer)
        {
            if (!Runtime.IsBuiltinResExist)
            {
                //没有内嵌资源的情况，不用处理
                return;
            }

            if (_vo.identifier.Equals(builtinResVer.identifier))
            {
                //和记录的标记一致，不用处理
                return;
            }

            if (string.IsNullOrEmpty(_vo.identifier))
            {
                //没有缓存信息，重置一下数据
                ClearVer();
            }
            else
            {
                Debug.Log($"[Zero][Cache][内嵌资源更新] res.json的identifier不一致,执行缓存资源清理。 [{_vo.identifier}] to [{builtinResVer.identifier}]");

                //从caches里删除所有的builtin里有记录的文件
                foreach (var item in builtinResVer.items)
                {
                    //移除版本号信息
                    RemoveVer(item.name);
                    //尝试删除文件
                    var path = FileUtility.CombinePaths(ZeroConst.WWW_RES_PERSISTENT_DATA_PATH, item.name);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        Debug.Log($"[Zero][Cache][内嵌资源更新] 删除过时的缓存文件: {path}");
                    }
                }
            }

            //更新标识符
            _vo.identifier = builtinResVer.identifier;

            //保存
            Save();
        }
    }
}