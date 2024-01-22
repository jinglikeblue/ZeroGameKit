using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    /// <summary>
    /// 插值信息数据
    /// </summary>
    public class InterpolationInfoVO
    {
        /// <summary>
        /// 距离最后一次GameCore刷新经过的时间
        /// </summary>
        public int deltaMS = 0;

        /// <summary>
        /// 插值因子
        /// </summary>
        public float lerpValue = 0;

        public override string ToString()
        {
            return $"[deltaMS:{deltaMS},lerpValue:{lerpValue}]";
        }
    }
}
