namespace Zero
{
    /// <summary>
    /// 热更DLL的执行方式
    /// </summary>
    public enum EILType
    {
        /// <summary>
        /// JIT(如果设备不支持JIT，会切换为ILRuntime)
        /// </summary>
        JIT,
        /// <summary>
        /// ILRuntime
        /// </summary>
        IL_RUNTIME,
        /// <summary>
        /// HuaTuo
        /// </summary>
        HUA_TUO,

        NONE,
    }
}