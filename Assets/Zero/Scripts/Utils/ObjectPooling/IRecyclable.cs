namespace Zero
{
    public interface IRecyclable
    {
        /// <summary>
        /// 被回收了
        /// </summary>
        void OnRecycled();

        /// <summary>
        /// 被遗弃了
        /// </summary>
        void OnDiscarded();
    }
}
