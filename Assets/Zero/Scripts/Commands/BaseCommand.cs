namespace Zero
{
    /// <summary>
    /// 逻辑指令基类
    /// </summary>
    public abstract class BaseCommand
    {
        /// <summary>
        /// 执行指令
        /// </summary>
        public virtual BaseCommand Excute()
        {
            ExcuteProcess();
            return this;
        }

        /// <summary>
        /// 终止指令
        /// </summary>
        public virtual void Terminate()
        {
            TerminateProcess();
        }

        /// <summary>
        /// 执行指令的处理
        /// </summary>
        protected abstract void ExcuteProcess();

        /// <summary>
        /// 终止指令的处理
        /// </summary>
        protected abstract void TerminateProcess();
    }
}