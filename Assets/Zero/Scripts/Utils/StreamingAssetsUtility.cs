using Jing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Zero
{
    /// <summary>
    /// StreamingAssets资源的工具类
    /// </summary>
    public static class StreamingAssetsUtility
    {
        public delegate void StreamingAssetsDataLoadedEvent(string path, byte[] bytes);
        public delegate void StreamingAssetsTextLoadedEvent(string path, string text);

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onChecked"></param>
        public static void CheckFileExist(string path, Action<bool> onChecked)
        {
            var handler = new StreamingAssetsFileExistCheckHandler(path);
            handler.onCompleted += onChecked;
            handler.Start();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onLoaded"></param>
        public static void LoadData(string path, StreamingAssetsDataLoadedEvent onLoaded)
        {
            var handler = new StreamingAssetsFileLoadHandler(path);
            handler.onCompleted += (h) =>
            {
                onLoaded?.Invoke(path, h.request.error == null ? h.request.downloadHandler.data: null);
            };
            handler.Start();
        }

        /// <summary>
        /// 加载文本
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onLoaded"></param>
        public static void LoadText(string path, StreamingAssetsTextLoadedEvent onLoaded)
        {
            var handler = new StreamingAssetsFileLoadHandler(path);
            handler.onCompleted += (h) =>
            {
                onLoaded?.Invoke(path, h.request.error == null ? h.request.downloadHandler.text: null);
            };
            handler.Start();
        }

        /// <summary>
        /// 加载AssetBundle
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundle(string path)
        {
            var ab = AssetBundle.LoadFromFile(path);
            return ab;
        }

        #region StreamingAssetsFileLoadHandler 操作类
        class StreamingAssetsFileLoadHandler
        {
            public string path { get; private set; }            

            public UnityWebRequest request { get; private set; }

            public event Action<StreamingAssetsFileLoadHandler> onCompleted;

            public StreamingAssetsFileLoadHandler(string path)
            {
                this.path = path;                
            }

            public void Start()
            {
                if (null != request)
                {
                    Debug.Log($"Start不能重复调用");
                    return;
                }
                request = UnityWebRequest.Get(path);
                var operation = request.SendWebRequest();
                operation.completed += OnCompleted;
            }

            private void OnCompleted(AsyncOperation obj)
            {
                onCompleted?.Invoke(this);
            }
        }
        #endregion

        #region StreamingAssetsFileExistCheckHandler 操作类
        class StreamingAssetsFileExistCheckHandler
        {
            class CheckHandler : DownloadHandlerScript
            {
                public CheckHandler() : base(new byte[1])
                {
                }

                public bool isFileExist { get; private set; } = false;

                protected override bool ReceiveData(byte[] data, int dataLength)
                {
                    if (dataLength > 0)
                    {
                        isFileExist = true;
                        return false;
                    }
                    return true;
                }
            }

            public string path { get; private set; }

            public event Action<bool> onCompleted;

            public bool isExist = false;

            CheckHandler _checkHandler;

            public StreamingAssetsFileExistCheckHandler(string path)
            {
                this.path = path;                

            }

            public void Start()
            {
                if (null != _checkHandler)
                {
                    Debug.LogWarning("Start不能重复调用，直接获取isExist属性即可");
                    return;
                }
                var www = UnityWebRequest.Get(path);
                _checkHandler = new CheckHandler();
                www.downloadHandler = _checkHandler;
                var operation = www.SendWebRequest();
                operation.completed += OnCompleted;
            }

            private void OnCompleted(AsyncOperation obj)
            {
                isExist = _checkHandler.isFileExist;
                onCompleted?.Invoke(isExist);
            }
        }
        #endregion
    }

}
