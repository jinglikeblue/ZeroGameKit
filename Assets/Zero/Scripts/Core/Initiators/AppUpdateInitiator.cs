using Jing;
using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    internal class AppUpdateInitiator : BaseInitiator
    {
        const string APK_INSTALL_FILE_EXT = ".apk";

        internal override void Start()
        {
            base.Start();

            if (Runtime.Ins.IsNeedNetwork)
            {
                CheckUpdate();
            }
            else
            {
                End();
            }
        }

        void CheckUpdate()
        {
            int result = CheckVersionCode(Application.version, Runtime.Ins.setting.client.version);
            if (result == -1)
            {
                try
                {
                    Uri updateURI = new Uri(Runtime.Ins.setting.client.url);
                    var url = string.IsNullOrEmpty(updateURI.Query) ? $"{updateURI.OriginalString}?ver={Runtime.Ins.setting.client.version}" : updateURI.AbsoluteUri;

                    if (updateURI.AbsolutePath.EndsWith(APK_INSTALL_FILE_EXT) && Application.platform == RuntimePlatform.Android)
                    {
                        //是APK安装文件
                        UpdateAPK(url);
                    }
                    else
                    {
                        //直接打开新版本对应的网页
                        Application.OpenURL(url);
                    }
                }
                catch
                {
                    End("更新App失败！");
                }
            }
            else
            {
                //不用更新
                End();
            }
        }

        /// <summary>
        /// 检查版本编码，如果本地号大于网络，则返回1，等于返回0，小于返回-1
        /// </summary>
        /// <param name="local"></param>
        /// <param name="net"></param>
        /// <returns></returns>
        protected int CheckVersionCode(string local, string net)
        {
            string[] locals = local.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string[] nets = net.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            int compareLength = locals.Length > nets.Length ? locals.Length : nets.Length;

            for (int i = 0; i < compareLength; i++)
            {
                int lc = 0;
                if (i < locals.Length)
                {
                    lc = int.Parse(locals[i]);
                }

                int nc = 0;
                if (i < nets.Length)
                {
                    nc = int.Parse(nets[i]);
                }

                if (lc > nc)
                {
                    return 1;
                }
                else if (lc < nc)
                {
                    return -1;
                }
            }

            return 0;
        }

        async void UpdateAPK(string apkUrl)
        {
            HttpDownloader loader = new HttpDownloader(apkUrl, FileUtility.CombinePaths(Runtime.Ins.localResDir, ZeroConst.ANDROID_APK_NAME));
            loader.Start();

            Debug.Log($"安装包保存路径:{loader.savePath}");
            while (!loader.isDone)
            {
                base.Progress(loader.loadedSize, loader.totalSize);
                await UniTask.NextFrame();
            }

            base.Progress(loader.totalSize, loader.totalSize);

            if (loader.error != null)
            {
                End(loader.error);
                return;
            }

#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                var apkInstallUtilityCls = new AndroidJavaClass("pieces.jing.zerolib.utilities.ApkInstallUtility");
                var installResult = apkInstallUtilityCls.CallStatic<bool>("install", loader.savePath);
                if (false == installResult)
                {
                    Debug.LogError("拉起安装程序失败");
                }
                else
                {
                    Debug.Log("拉起安装程序");
                }
            }
            else
            {
                Debug.Log("真机环境下，会拉起安装Apk！");
            }
#endif
        }
    }
}