using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 本地数据模块。用于将一些数据序列化到硬盘。和PlayerPrefs功能类似，区别是可以从I/O目录中找到该类保存的数据。
    /// </summary>
    public class LocalDataModel
    {
        /// <summary>
        /// 数据加密Key
        /// </summary>
        const string DATA_ENCRYPT_KEY = "zero";

        /// <summary>
        /// 是否允许加密
        /// </summary>
        const bool ENCRYPT_ENABLE = false;

        const string FILE_NAME = "local_data.zero.json";
        /// <summary>
        /// 本地数据对象
        /// </summary>
        public class VO
        {
            public Dictionary<string, string> localValueDic = new Dictionary<string, string>();
        }

        VO _vo;
        string _path;

        public LocalDataModel()
        {
            _path = FileUtility.CombinePaths(Runtime.generateFilesDir, FILE_NAME);

            if (File.Exists(_path))
            {
                var content = File.ReadAllText(_path);
                //读取已有的数据
                _vo = Json.ToObject<VO>(content);
            }
            else
            {
                //新数据初始化
                _vo = new VO();                
            }
        }

        /// <summary>
        /// 保存数据到本地
        /// </summary>
        void Save2Local()
        {           
            string json = Json.ToJson(_vo);
            File.WriteAllText(_path, json);
        }

        /// <summary>
        /// 添加一个存储到本地的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, string value)
        {
            if (ENCRYPT_ENABLE)
            {
                key = AESHelper.Encrypt(key, DATA_ENCRYPT_KEY);
                value = AESHelper.Encrypt(value, DATA_ENCRYPT_KEY);
            }
            _vo.localValueDic[key] = value;
            Save2Local();
        }

        /// <summary>
        /// 读取一个存储到本地的字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ReadValue(string key)
        {
            if (ENCRYPT_ENABLE)
            {
                key = AESHelper.Encrypt(key, DATA_ENCRYPT_KEY);
            }
            if (_vo.localValueDic.ContainsKey(key))
            {                
                var value = _vo.localValueDic[key];
                if (ENCRYPT_ENABLE)
                {
                    value = AESHelper.Decrypt(value, DATA_ENCRYPT_KEY);
                }
                return value;
            }
            return null;
        }

        /// <summary>
        /// 移除一个存储到本地的字段
        /// </summary>
        /// <param name="key"></param>
        public void RemoveValue(string key)
        {
            if(_vo.localValueDic.ContainsKey(key))
            {
                _vo.localValueDic.Remove(key);
                Save2Local();
            }
        }

        /// <summary>
        /// 清除所有存储到本地的字段
        /// </summary>
        public void ClearValues()
        {
            _vo.localValueDic.Clear();
            Save2Local();
        }
    }
}
