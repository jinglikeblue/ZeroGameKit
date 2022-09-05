using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zero
{
    /// <summary>
    /// Web下载基类
    /// </summary>

    public abstract class BaseWebDownloader
    {        
        /// <summary>
        /// 已下载大小
        /// </summary>
        public long downloadedSize { get; protected set; } = 0;

        /// <summary>
        /// 总大小
        /// </summary>
        public long totalSize { get; protected set; } = 0;

        /// <summary>
        /// 下载进度
        /// </summary>
        public float progress
        {
            get
            {
                if(totalSize == 0)
                {
                    return 0;
                }

                float v = (float)downloadedSize / totalSize;                   
                return v;
            }            
        }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool isDone { get; protected set; } = false;

        /// <summary>
        /// 错误原因(null为无错误)
        /// </summary>
        public string error { get; protected set; } = null;

        /// <summary>
        /// 是否是取消了下载
        /// </summary>
        public bool isCanceled { get; protected set; } = false;

        public void Cancel()
        {
            isCanceled = true;
        }
    }
}
