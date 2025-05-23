﻿using Jing;
using System.IO;

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
                _vo.items = new ResVerVO.Item[0];
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
            string json = Json.ToJson(_vo);
            File.WriteAllText(_path, json);
        }

    }
}
