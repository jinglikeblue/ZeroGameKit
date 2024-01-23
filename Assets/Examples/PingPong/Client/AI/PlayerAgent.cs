using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    /// <summary>
    /// 角色代理
    /// </summary>
    public class PlayerAgent
    {
        /// <summary>
        /// 获取角色的输入
        /// </summary>
        /// <returns></returns>
        public PlayerInput GetInput()
        {
            return PlayerInput.Default;
        }
    }
}
