namespace Zero
{
    /// <summary>
    /// 热更入口类
    /// </summary>
    public static class HotEntrance
    {
        /// <summary>
        /// 当前的Zero.Main实例
        /// </summary>
        public static Main CurrentMain { get; private set; }

        /// <summary>
        /// 热更代码入口
        /// </summary>
        public static void Startup()
        {
            //注册类型名到预制件的隐射
            ViewFactory.PathFindFunc = R.GetPath;

            CurrentMain = new Main();
        }
    }
}