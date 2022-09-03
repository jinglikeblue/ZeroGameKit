using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zero
{
    public abstract class BaseWriteableResVerModel : ResVerModel
    {
        /// <summary>
        /// 设置文件版本号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public abstract void SetVerAndSave(string name, string version);

        /// <summary>
        /// 移除指定文件的版本信息
        /// </summary>
        /// <returns>The ver.</returns>
        /// <param name="name">Name.</param>
        public abstract void RemoveVerAndSave(string name);

        /// <summary>
        /// 清理所有版本信息
        /// </summary>
        public abstract void ClearVerAndSave();
    }
}
