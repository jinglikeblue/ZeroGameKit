using Jing;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Zero;

namespace Zero
{
    /// <summary>
    /// 热更Files管理工具，所有的加载都是异步执行的(因为使用内嵌资源时，只能用UnityWebRequest的方式读取)
    /// </summary>
    public class HotFilesMgr : BaseSingleton<HotFilesMgr>
    {
        /// <summary>
        /// 热更文件加载器
        /// </summary>
        public class Loader 
        {
            public enum EDataMode
            {
                BYTES,
                TEXT
            }

            /// <summary>
            /// 加载进度
            /// </summary>
            public float progress { get; protected set; } = 0;

            /// <summary>
            /// 已下载大小
            /// </summary>
            //public long loadedSize { get; protected set; } = 0;

            /// <summary>
            /// 总大小
            /// </summary>
            //public long totalSize { get; protected set; } = 0;

            /// <summary>
            /// 是否完成
            /// </summary>
            public bool isDone { get; protected set; } = false;

            /// <summary>
            /// 错误原因(null为无错误)
            /// </summary>
            public string error { get; protected set; } = null;

            /// <summary>
            /// 是否是取消了下载
            /// </summary>
            public bool isCanceled { get; protected set; } = false;

            /// <summary>
            /// 加载到的数据
            /// </summary>
            public byte[] bytes { get; private set; } = null;

            /// <summary>
            /// 加载到的文本
            /// </summary>
            public string text { get; private set; } = null;

            /// <summary>
            /// 资源的模式
            /// </summary>
            public EDataMode dataMode { get; private set; }

            /// <summary>
            /// 文件路径
            /// </summary>
            public string path { get; private set; } = null;

            string GetPersistentPath()
            {
                return FileUtility.CombinePaths(RootDir, path);
            }

            string GetStreamingAssetsPath()
            {
                return FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, ZeroConst.FILES_DIR_NAME, path);
            }

            internal Loader(string path, EDataMode dataMode)
            {
                this.path = path;
                this.dataMode = dataMode;

                switch (Runtime.Ins.BuiltinResMode)
                {
                    case EBuiltinResMode.HOT_PATCH:                        
                        if (!LoadFromCatch())
                        {
                            //检查是否有内嵌资源，有的话从内嵌资源提取
                            ILBridge.Ins.StartCoroutine(this, LoadFromStreamingAssets());
                        }
                        break;
                    case EBuiltinResMode.ONLY_USE:
                        ILBridge.Ins.StartCoroutine(this, LoadFromStreamingAssets());
                        break;
                }      
            }  
            
            bool LoadFromCatch()
            {
                ///首先从缓存里读取
                var absolutePath = GetPersistentPath();
                if (File.Exists(absolutePath))
                {
                    switch (dataMode)
                    {
                        case EDataMode.BYTES:
                            bytes = File.ReadAllBytes(absolutePath);
                            break;
                        case EDataMode.TEXT:
                            text = File.ReadAllText(absolutePath);
                            break;
                    }
                    Debug.Log(LogColor.Zero2($"HotFiles读取成功[IO]: {absolutePath}"));
                    Complete();
                    return true;
                }

                return false;
            }

            IEnumerator LoadFromStreamingAssets()
            {
                var absolutePath = GetStreamingAssetsPath();
                var uwr = UnityWebRequest.Get(absolutePath);
                uwr.SendWebRequest();                
                while (false == uwr.isDone)
                {
                    if (isCanceled)
                    {
                        uwr.Abort();
                        Complete();
                        yield break;
                    }

                    progress = uwr.downloadProgress;
                    yield return 0;
                }

                if(uwr.error != null)
                {
                    Complete($"[{absolutePath}] 文件不存在");
                    yield break;
                }

                switch (dataMode)
                {
                    case EDataMode.BYTES:
                        bytes = uwr.downloadHandler.data;
                        break;
                    case EDataMode.TEXT:
                        text = uwr.downloadHandler.text;
                        break;
                }
                Debug.Log(LogColor.Zero2($"HotFiles读取成功[StreamingAssets]: {absolutePath}"));
                Complete();                
            }

            void Complete(string error = null)
            {
                if (error == null)
                {
                    progress = 1;                    
                }
                else
                {
                    this.error = error;
                }
                            
                isDone = true;
            }

            public void Cancel()
            {
                if (false == isDone)
                {
                    isCanceled = true;
                }
            }

            /// <summary>
            /// 根目录
            /// </summary>
            static public string RootDir
            {
                get
                {
                    string path = null;
                    switch (Runtime.Ins.HotResMode)
                    {
                        case EHotResMode.NET_ASSET_BUNDLE:
                            path = FileUtility.CombineDirs(false, ZeroConst.WWW_RES_PERSISTENT_DATA_PATH, ZeroConst.FILES_DIR_NAME);
                            break;
                        case EHotResMode.LOCAL_ASSET_BUNDLE:
                            path = FileUtility.CombineDirs(false, ZeroConst.PUBLISH_RES_ROOT_DIR, ZeroConst.FILES_DIR_NAME);
                            break;
                        case EHotResMode.ASSET_DATA_BASE:
                            //该种开发模式下，直接从Asset/@Files取文件
                            path = FileUtility.CombineDirs(false, ZeroConst.HOT_FILES_ROOT_DIR);
                            break;
                    }
                    return path;
                }
            }

            
        }


        public Loader LoadBytes(string path)
        {
            var loader = new Loader(path, Loader.EDataMode.BYTES);
            return loader;
        }

        public Loader LoadText(string path)
        {
            var loader = new Loader(path, Loader.EDataMode.TEXT);
            return loader;
        }        

        public string GetAbsolutePath(string path)
        {
            if (Runtime.Ins.IsHotResEnable)
            {
                var hotResPath = FileUtility.CombinePaths(Loader.RootDir, path);
                if (File.Exists(hotResPath))
                {
                    return hotResPath;
                }
            }

            if (Runtime.Ins.IsBuildinResExist)
            {
                var builtinResPath = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, ZeroConst.FILES_DIR_NAME, path);
                return builtinResPath;
            }
            return null;
        }

        public override void Destroy()
        {
            
        }

        protected override void Init()
        {

        }
    }
}
