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
        public static void Start(RuntimeModel runtime)
        {
            var world = new WorldEntity();
            world.state = EWorldState.PLAYING;

            runtime.vo.runtimeFrameData.world = world;
        }

        public static void Update(RuntimeModel runtime)
        {
            //移动系统更新
            MoveSystem.Update(runtime);
            //击球系统更新
            ShotSystem.Update(runtime);
        }

        /// <summary>
        /// 所有的Update执行后的更新
        /// </summary>
        /// <param name="runtime"></param>
        public static void LateUpdate(RuntimeModel runtime)
        {
            //判决系统更新
            JudgeSystem.Update(runtime);
        }

        public static void End(RuntimeModel runtime)
        {

        }
    }
}
