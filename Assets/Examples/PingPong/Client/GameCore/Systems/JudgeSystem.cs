using Zero;

namespace PingPong
{
    /// <summary>
    /// 裁判系统(胜负判断)
    /// </summary>
    public static class JudgeSystem
    {
        public static void Update(RuntimeModel runtime)
        {
            var world = runtime.vo.runtimeFrameData.world;

            var ball = world.ball;

            if (ball.position.y < world.size.top)
            {
                GameOver(world, 0);
                return;
            }

            if (ball.position.y > world.size.bottom)
            {
                GameOver(world, 1);
                return;
            }
        }

        static void GameOver(WorldEntity world, int winner)
        {
            world.winner = winner;
            world.state = EWorldState.END;
            GUIDebugInfo.SetInfo("World", $"state {world.state}");
        }
    }
}
