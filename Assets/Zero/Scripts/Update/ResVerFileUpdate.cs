﻿using Jing;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 网络端版本号描述文件加载
    /// </summary>
    public class ResVerFileUpdate
    {
        Action _onLoaded;
        Action<string> _onError;

        string _localPath;

        public void Start(Action onLoaded, Action<string> onError)
        {
            Debug.Log(Log.Zero1("「ResVerFileUpdate」资源版本号文件更新检查..."));
            _onLoaded = onLoaded;
            _onError = onError;
            _localPath = FileUtility.CombinePaths(Runtime.Ins.localResDir , ZeroConst.RES_JSON_FILE_NAME);
            if (Runtime.Ins.IsLoadAssetsFromNet)
            {
                ILBridge.Ins.StartCoroutine(Update());
            }
            else
            {
                Runtime.Ins.netResVer = new ResVerModel(LoadLocalFile());
                _onLoaded();
            }
        }

        ResVerVO LoadLocalFile()
        {
            string jsonStr = File.ReadAllText(_localPath);
            ResVerVO vo = LitJson.JsonMapper.ToObject<ResVerVO>(jsonStr);
            return vo;
        }

        IEnumerator Update()
        {
            Downloader loader = new Downloader(FileUtility.CombinePaths(Runtime.Ins.netResDir, ZeroConst.RES_JSON_FILE_NAME), _localPath, DateTime.UtcNow.ToFileTimeUtc().ToString());
            while (false == loader.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            if (null != loader.error)
            {
                Debug.LogErrorFormat(loader.error);
                if (null != _onError)
                {
                    _onError.Invoke(loader.error);
                }
                yield break;
            }
            
            loader.Dispose();

            Runtime.Ins.netResVer = new ResVerModel(LoadLocalFile());
            _onLoaded();
            yield break;
        }
    }
}
