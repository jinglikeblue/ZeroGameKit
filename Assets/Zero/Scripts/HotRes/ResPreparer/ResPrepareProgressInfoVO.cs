namespace Zero
{
    /// <summary>
    /// 资源预加载进度信息
    /// </summary>
    public class ResPrepareProgressInfoVO
    {
        /// <summary>
        /// 所有要预载的项
        /// </summary>
        public ResVerVO.Item[] prepareItems;

        /// <summary>
        /// 正在预载的索引
        /// </summary>
        public int preparingIndex = -1;

        /// <summary>
        /// 当前正在预载的项
        /// </summary>
        public ResVerVO.Item PreparingItem => prepareItems[preparingIndex];

        /// <summary>
        /// 当前操作路径已预载大小
        /// </summary>
        public long unitLoadedSize = 0;

        /// <summary>
        /// 当前操作路径总大小
        /// </summary>
        public long UnitTotalSize => PreparingItem.size;

        /// <summary>
        /// 已预载数量
        /// </summary>
        public int loadedCount = 0;

        /// <summary>
        /// 总预载数量
        /// </summary>
        public int TotalCount => prepareItems.Length;

        /// <summary>
        /// 已预载大小
        /// </summary>
        public long loadedSize = 0;

        /// <summary>
        /// 要预载的总大小
        /// </summary>
        public long totalSize;
        
        /// <summary>
        /// 进度
        /// </summary>
        public float Progress => (float)loadedSize / totalSize;

        public override string ToString()
        {
            if (TotalCount == 0)
            {
                return "[PrepareProgressInfo] 空数据";
            }

            return $"[PrepareProgressInfo][已预载数量: {loadedCount}/{TotalCount}][已预载字节数: {loadedSize}/{totalSize}][预载中：{PreparingItem.name}, 字节数: {unitLoadedSize}/{UnitTotalSize}]";
        }
    }
}