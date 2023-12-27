using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    /// <summary>
    /// 中心系统
    /// </summary>
    public static class CenterSystem
    {
        public static void Update(RuntimeModel runtime)
        {
            //移动系统更新
            MoveSystem.Update(runtime);
            //击球系统更新
            ShotSystem.Update(runtime);
            //判决系统更新
            JudgeSystem.Update(runtime);
        }
    }
}
