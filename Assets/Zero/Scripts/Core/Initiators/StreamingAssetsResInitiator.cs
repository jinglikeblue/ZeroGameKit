using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Zero
{
    /// <summary>
    /// 内嵌资源创始器
    /// </summary>
    internal class StreamingAssetsResInitiator : BaseInitiator
    {
        /// <summary>
        /// 是否有内嵌资源
        /// </summary>
        public bool IsResExist
        {
            get
            {
                if (settingVO == null || resVerVO == null || scriptDllBytes == null || scriptPdbBytes == null)
                {
                    return false;
                }

                return true;
            }
        }

        public SettingVO settingVO { get; private set; } = null;

        public ResVerVO resVerVO { get; private set; } = null;

        public byte[] scriptDllBytes { get; private set; } = null;

        public byte[] scriptPdbBytes { get; private set; } = null;

        internal StreamingAssetsResInitiator()
        {
        }

        internal override void Start()
        {
            base.Start();
            LoadSettingJson();
        }

        async void LoadSettingJson()
        {
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, ZeroConst.SETTING_FILE_NAME);
            var uwr = UnityWebRequest.Get(path);

            try
            {
                await uwr.SendWebRequest().ToUniTask();
            }
            catch (Exception e)
            {
                Debug.Log(LogColor.Red(e.ToString()));
            }

            if (uwr.error != null)
            {
                //加载不到表示没有内嵌资源
                End();
            }
            else
            {
                settingVO = Json.ToObject<SettingVO>(uwr.downloadHandler.text);
                LoadResJson();
            }
        }

        async void LoadResJson()
        {
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, ZeroConst.RES_JSON_FILE_NAME);
            var uwr = UnityWebRequest.Get(path);

            try
            {
                await uwr.SendWebRequest().ToUniTask();
            }
            catch (Exception e)
            {
                Debug.Log(LogColor.Red(e.ToString()));
            }

            if (uwr.error != null)
            {
                End(uwr.error);
            }
            else
            {
                resVerVO = Json.ToObject<ResVerVO>(uwr.downloadHandler.text);
                LoadScripts();
            }
        }

        async void LoadScripts()
        {
            var dllPath = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".dll");

            var uwr = UnityWebRequest.Get(dllPath);

            try
            {
                await uwr.SendWebRequest().ToUniTask();
            }
            catch (Exception e)
            {
                Debug.Log(LogColor.Red(e.ToString()));
            }

            if (uwr.error != null)
            {
                End(uwr.error);
            }
            else
            {
                scriptDllBytes = uwr.downloadHandler.data;
            }

            var pdbPath = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".pdb");

            uwr = UnityWebRequest.Get(pdbPath);
            
            try
            {
                await uwr.SendWebRequest().ToUniTask();
            }
            catch (Exception e)
            {
                Debug.Log(LogColor.Red(e.ToString()));
            }

            if (uwr.error != null)
            {
                End(uwr.error);
            }
            else
            {
                scriptPdbBytes = uwr.downloadHandler.data;
            }

            End();
        }
    }
}