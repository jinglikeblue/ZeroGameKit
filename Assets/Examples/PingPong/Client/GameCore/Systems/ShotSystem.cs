using Jing.FixedPointNumber;
using System.Linq;

namespace PingPong
{
    /// <summary>
    /// 击球系统
    /// </summary>
    public static class ShotSystem
    {
        public static void Update(RuntimeModel runtime)
        {
            var world = runtime.vo.runtimeFrameData.world;

            var ball = world.ball;
            
            for(int i = 0; i < world.players.Length; i++)
            {
                var player = world.players[i];
                var playerRect = PlayerUtility.CreatePlayerBody(player);
                var ballRound = BallUtility.CreateBallBody(ball);                
                if (Geometry.CheckIntersect(playerRect, ballRound))
                {
                    //击打球，球速度翻转
                    ball.speed.y *= -1;
                }
            }
        }
    }
}
