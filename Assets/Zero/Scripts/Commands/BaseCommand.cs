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
        public BaseCommand Execute()
        {
            ExecuteProcess();
            return this;
        }

        /// <summary>
        /// 终止指令
        /// </summary>
        public void Terminate()
        {
            TerminateProcess();
        }       

        /// <summary>
        /// 执行指令的处理
        /// </summary>
        protected abstract void ExecuteProcess();

        /// <summary>
        /// 终止指令的处理
        /// </summary>
        protected abstract void TerminateProcess();
    }
}
