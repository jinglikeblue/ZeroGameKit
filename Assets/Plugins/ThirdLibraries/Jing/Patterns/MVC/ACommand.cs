namespace Jing
{
    /// <summary>
    /// 逻辑指令基类
    /// </summary>
    public abstract class ACommand
    {
        /// <summary>
        /// 执行指令
        /// </summary>
        public virtual ACommand Excute()
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
