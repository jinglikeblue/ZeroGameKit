namespace Zero
{
    /// <summary>
    /// 内嵌资源使用模式
    /// </summary>
    public enum EBuiltinResMode
    {
        /// <summary>
        /// 热补丁模式（推荐）。优先使用通过网络更新到的资源，其次使用内嵌的资源
        /// [依赖网络]
        /// </summary>
        HOT_PATCH,

        /// <summary>
        /// 仅使用内嵌资源（一般用来制作不联网的单机游戏）。
        /// 该模式下不使用DLL来运行，而是直接执行项目内嵌的代码（效率更高）
        /// [不依赖网络]
        /// </summary>
        ONLY_USE,
    }

    /// <summary>
    /// 热更资源使用模式
    /// </summary>
    public enum EHotResMode
    {
        /// <summary>
        /// 从网络资源目录获取资源（最终发布时使用的模式）
        /// [依赖网络]
        /// </summary>            
        NET_ASSET_BUNDLE,
        /// <summary>
        /// 从本地资源目录获取资源（测试AB资源加载时使用的模式）
        /// [不依赖网络]
        /// </summary>            
        LOCAL_ASSET_BUNDLE,
        /// <summary>
        /// 使用AssetDataBase接口加载资源（开发时使用的模式）
        /// [不依赖网络]
        /// </summary>
        ASSET_DATA_BASE,
    }



}