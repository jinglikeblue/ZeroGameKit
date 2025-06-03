using Jing;
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Zero
{
    /// <summary>
    /// StreamingAssets资源的工具类
    /// </summary>
    public static class StreamingAssetsUtility
    {
        /// <summary>
        /// 通过相对路径获取绝对路径
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string MakeAbsolutePath(string relativePath)
        {
            if (!relativePath.StartsWith(ZeroConst.STREAMING_ASSETS_PATH))
            {
                return FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_PATH, relativePath);
            }

            return relativePath;
        }

        /// <summary>
        /// 检查文件是否存在(同步方法)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        private static async UniTask<bool> CheckFileExist(string path, bool isAsync)
        {
            path = MakeAbsolutePath(path);
            var www = UnityWebRequest.Get(path);
            var tempFilePath = FileUtility.CombinePaths(Application.temporaryCachePath, $"streaming_assets_check_{Path.GetFileNameWithoutExtension(path)}.bytes");
            www.downloadHandler = new DownloadHandlerFile(tempFilePath);
            www.SendWebRequest();

            // Debug.Log($"[CheckStreamingAssetsFileExist] ResponseCode: {www.responseCode} DownloadedSize: {www.downloadedBytes}");
            while (false == (www.isDone || www.downloadedBytes > 0))
            {
                if (isAsync)
                {
                    await UniTask.NextFrame();
                }
            }

            www.Abort();
            // Debug.Log($"[CheckStreamingAssetsFileExist] ResponseCode: {www.responseCode} DownloadedSize: {www.downloadedBytes}");

            bool isExist = www.downloadedBytes > 0;

            if (isExist)
            {
                Debug.Log($"[CheckStreamingAssetsFileExist] 存在: {path}");
            }
            else
            {
                Debug.Log($"[CheckStreamingAssetsFileExist] 不存在({www.error}): {path}");
            }

            //清理WWW
            www.downloadHandler.Dispose();
            www.Dispose();

            //清理临时文件
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            return isExist;
        }

        /// <summary>
        /// 检查文件是否存在(同步方法)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckFileExist(string path)
        {
            var task = CheckFileExist(path, false);
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// 检查文件是否存在(同步方法)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async UniTask<bool> CheckFileExistAsync(string path)
        {
            return await CheckFileExist(path, true);
        }

        #region 同步加载方式

        /// <summary>
        /// 加载文本(同步方式)
        /// PS:过大的文件不要用这个
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="timeoutSeconds">超时限制，如果占用线程超过这个时间，则返回null</param>
        /// <returns></returns>
        public static string LoadText(string path, float timeoutSeconds = 60)
        {
            var www = LoadSync(path, timeoutSeconds);
            return null == www ? null : www.downloadHandler.text;
        }

        /// <summary>
        /// 加载数据(同步方式)
        /// PS:过大的文件不要用这个
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="timeoutSeconds">超时限制，如果占用线程超过这个时间，则返回null</param>
        /// <returns></returns>
        public static byte[] LoadData(string path, float timeoutSeconds = 60)
        {
            var www = LoadSync(path, timeoutSeconds);
            return null == www ? null : www.downloadHandler.data;
        }

        static UnityWebRequest LoadSync(string path, float timeoutSeconds)
        {
            path = MakeAbsolutePath(path);
            var www = UnityWebRequest.Get(path);
            www.SendWebRequest();

            var startTime = DateTime.Now;
            while (!www.isDone)
            {
                var tn = DateTime.Now - startTime;
                if (tn.TotalSeconds > timeoutSeconds)
                {
                    //超时处理
                    Debug.LogError($"[StreamingAssetsUtility][Error] {path}");
                    Debug.LogError($"[StreamingAssetsUtility][Error] 加载失败！超过限定时间:{timeoutSeconds}秒");
                    return null;
                }
            }

            if (www.error != null)
            {
                Debug.LogError($"[StreamingAssetsUtility][Error] {path}");
                Debug.LogError(www.error);
                return null;
            }

            return www;
        }

        #endregion

        private static async UniTask<UnityWebRequest> LoadAsync(string path)
        {
            path = MakeAbsolutePath(path);
            var www = UnityWebRequest.Get(path);
            www.SendWebRequest();
            while (false == www.isDone)
            {
                await UniTask.NextFrame();
            }
            
            if (www.error != null)
            {
                Debug.LogError($"[StreamingAssetsUtility][Error] {path}");
                Debug.LogError(www.error);
            }

            return www;
        }

        /// <summary>
        /// 加载数据(异步方式)
        /// </summary>
        /// <param name="path"></param>
        public static async UniTask<byte[]> LoadDataAsync(string path)
        {
            var www = await LoadAsync(path);
            return www.error == null ? www.downloadHandler.data : null;
        }


        /// <summary>
        /// 加载文本(异步方式)
        /// </summary>
        /// <param name="path"></param>
        public static async UniTask<string> LoadTextAsync(string path)
        {
            var www = await LoadAsync(path);
            return www.error == null ? www.downloadHandler.text : null;
        }

        /// <summary>
        /// 加载AssetBundle
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundle(string path)
        {
            path = MakeAbsolutePath(path);
            var ab = AssetBundle.LoadFromFile(path);
            return ab;
        }
    }
}