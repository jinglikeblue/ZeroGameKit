using System.Text;
using Cysharp.Threading.Tasks;
using Jing;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 内嵌资源检查器
    /// </summary>
    public class BuiltinInitiator : BaseInitiator
    {
        public SettingVO Setting { get; private set; } = null;

        public ResVerVO ResVer { get; private set; } = null;

        public byte[] DllBytes { get; private set; } = null;

        public byte[] PdbBytes { get; private set; } = null;

        /// <summary>
        /// setting.json是否存在
        /// </summary>
        public bool IsBuiltinSettingExist => Setting != null;

        /// <summary>
        /// res.json是否存在。该文件重要，记录了所有内嵌资源的版本号信息
        /// </summary>
        public bool IsBuiltinResVerExist => ResVer != null;

        /// <summary>
        /// 脚本dll是否存在
        /// </summary>
        public bool IsBuiltinDllExist => DllBytes != null;

        internal override async UniTask<string> StartAsync(InitiatorProgress onProgress = null)
        {
            Setting = await LoadJson<SettingVO>(ZeroConst.SETTING_FILE_NAME);
            ResVer = await LoadJson<ResVerVO>(ZeroConst.RES_JSON_FILE_NAME);
            var dllPath = FileUtility.CombinePaths(ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".dll");
            DllBytes = await Res.LoadFromStreamingAssetsAsync(dllPath);
            var pdbPath = FileUtility.CombinePaths(ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".pdb");
            PdbBytes = await Res.LoadFromStreamingAssetsAsync(pdbPath);

            Debug.Log(LogColor.Zero2($"[Zero][Initiator] 内嵌资源[{ZeroConst.SETTING_FILE_NAME}]是否存在: {IsBuiltinSettingExist} "));
            Debug.Log(LogColor.Zero2($"[Zero][Initiator] 内嵌资源[{ZeroConst.RES_JSON_FILE_NAME}]是否存在: {IsBuiltinResVerExist} "));
            Debug.Log(LogColor.Zero2($"[Zero][Initiator] 内嵌资源[{dllPath}]是否存在: {IsBuiltinDllExist} "));
            Debug.Log(LogColor.Zero2($"[Zero][Initiator] 内嵌资源[{pdbPath}]是否存在: {PdbBytes != null} "));

            return null;
        }

        async UniTask<T> LoadJson<T>(string path)
        {
            var bytes = await Res.LoadFromStreamingAssetsAsync(path);
            if (null == bytes)
            {
                return default;
            }

            var jsonString = Encoding.UTF8.GetString(bytes);
            return Json.ToObject<T>(jsonString);
        }
    }
}