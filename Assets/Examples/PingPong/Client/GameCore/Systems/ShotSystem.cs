using Jing.FixedPointNumber;
using Zero;

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

            for (int i = 0; i < world.players.Length; i++)
            {
                var player = world.players[i];
                var playerRect = PlayerUtility.CreatePlayerBody(player);
                var ballRound = BallUtility.CreateBallBody(ball);
                if (Geometry.CheckIntersect(playerRect, ballRound))
                {
                    //击打球，球速度翻转
                    //ball.speed.y *= -1;
                    Number aaa = (Number)1;
                    int bbb = (int)aaa;
                    #region 计算球的竖向速度
                    if (i == 0)
                    {
                        ball.speed.y = Math.Abs(ball.speed.y);
                    }
                    else if (i == 1)
                    {
                        ball.speed.y = -Math.Abs(ball.speed.y);
                    }
                    #endregion

                    #region 计算求的横向速度
                    //计算球的X轴距离Player的中心点的距离
                    var distanceX = ball.position.x - player.position.x;
                    var halfSize = Define.PLAYER_SIZE.x >> 1;
                    var pow = distanceX / halfSize;
                    GUIDebugInfo.SetInfo($"Player_{i} Hit", $"distance {distanceX}, pow {pow}");
                    pow = Math.Clamp(pow, new Number(-1), (Number)1);
                    var speedX = pow * 15;
                    ball.speed.x = speedX;
                    GUIDebugInfo.SetInfo("Ball", $"speed {ball.speed}");
                    #endregion
                    return;
                }
            }
        }
    }
}
