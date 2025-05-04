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
                if (settingVO == null || resVerVO == null || scriptDllBytes == null)
                {
                    return false;
                }

                return true;
            }
        }

        public SettingVO settingVO { get; private set; } = null;

        public ResVerVO resVerVO { get; private set; } = null;

        public byte[] scriptDllBytes { get; private set; } = null;

        /// <summary>
        /// 是否内嵌DLL存在
        /// </summary>
        public bool IsBuiltinDllExist => scriptDllBytes != null;

        public byte[] scriptPdbBytes { get; private set; } = null;

        internal StreamingAssetsResInitiator()
        {
        }

        internal override async void Start()
        {
            base.Start();

            await LoadSettingJson();
            await LoadResJson();
            await LoadScripts();

            End();
        }

        async UniTask LoadSettingJson()
        {
            var bytes = await HotRes.LoadFromStreamingAssets(ZeroConst.SETTING_FILE_NAME);
            if (null == bytes)
            {
                Debug.Log(LogColor.Zero2($"[Zero][Launcher] 内嵌资源不存在: {ZeroConst.SETTING_FILE_NAME}"));
                return;
            }
            
            var jsonString = Encoding.UTF8.GetString(bytes);
            settingVO = Json.ToObject<SettingVO>(jsonString);
        }

        async UniTask LoadResJson()
        {
            var bytes = await HotRes.LoadFromStreamingAssets(ZeroConst.RES_JSON_FILE_NAME);
            if (null == bytes)
            {
                Debug.Log(LogColor.Zero2($"[Zero][Launcher] 内嵌资源不存在: {ZeroConst.RES_JSON_FILE_NAME}"));
                return;
            }
            
            var jsonString = Encoding.UTF8.GetString(bytes);
            resVerVO = Json.ToObject<ResVerVO>(jsonString);
        }

        async UniTask LoadScripts()
        {
            var dllPath = FileUtility.CombinePaths(ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".dll");
            scriptDllBytes = await HotRes.LoadFromStreamingAssets(dllPath);

            var pdbPath = FileUtility.CombinePaths(ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".pdb");
            scriptPdbBytes = await HotRes.LoadFromStreamingAssets(pdbPath);
        }
    }
}