using Cysharp.Threading.Tasks;

namespace Zero
{
    /// <summary>
    /// 逻辑指令基类
    /// </summary>
    public abstract class BaseAsyncCommand
    {
        /// <summary>
        /// 执行指令
        /// </summary>
        public UniTask Execute()
        {
            return ExecuteProcess();
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
        protected abstract UniTask ExecuteProcess();

        /// <summary>
        /// 终止指令的处理
        /// </summary>
        protected abstract void TerminateProcess();
    }
}