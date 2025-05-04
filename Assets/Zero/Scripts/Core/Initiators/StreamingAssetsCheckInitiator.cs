using Cysharp.Threading.Tasks;
using Jing;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 内嵌资源检查器
    /// </summary>
    public class StreamingAssetsCheckInitiator : BaseInitiator
    {
        /// <summary>
        /// setting.json是否存在
        /// </summary>
        public bool IsBuiltinSettingExist { get; private set; } = false;

        /// <summary>
        /// res.json是否存在。该文件重要，记录了所有内嵌资源的版本号信息
        /// </summary>
        public bool IsBuiltinResVerExist { get; private set; } = false;

        /// <summary>
        /// 脚本dll是否存在
        /// </summary>
        public bool IsBuiltinDllExist { get; private set; } = false;

        internal override async UniTask StartAsync()
        {
            await base.StartAsync();
            IsBuiltinSettingExist = await HotRes.CheckStreamingAssetsExist(ZeroConst.SETTING_FILE_NAME);
            IsBuiltinResVerExist = await HotRes.CheckStreamingAssetsExist(ZeroConst.RES_JSON_FILE_NAME);
            var dllPath = FileUtility.CombinePaths(ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".dll");
            IsBuiltinDllExist = await HotRes.CheckStreamingAssetsExist(dllPath);

            Debug.Log(LogColor.Zero2($"[Zero][Initiator] 内嵌资源[{ZeroConst.SETTING_FILE_NAME}]是否存在: {IsBuiltinSettingExist} "));
            Debug.Log(LogColor.Zero2($"[Zero][Initiator] 内嵌资源[{ZeroConst.RES_JSON_FILE_NAME}]是否存在: {IsBuiltinResVerExist} "));
            Debug.Log(LogColor.Zero2($"[Zero][Initiator] 内嵌资源[{dllPath}]是否存在: {IsBuiltinDllExist} "));

            End();
        }
    }
}