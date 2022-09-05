namespace Zero
{
    /// <summary>
    /// 热更DLL的执行方式
    /// </summary>
    public enum EILType
    {
        /// <summary>
        /// ILRuntime
        /// </summary>
        IL_RUNTIME,
        /// <summary>
        /// HybridClr，也叫WoLong，也叫HuaTuo
        /// </summary>
        HYBRID_CLR,
        /// <summary>
        /// 程序与反射执行
        /// </summary>
        JIT,

        NONE,
    }
}