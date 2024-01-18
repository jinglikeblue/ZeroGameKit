using Jing.FixedPointNumber;

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
            world.size = new Rect(Vector2.ZERO, Define.WORLD_SIZE);
            world.size.center = Vector2.ZERO;            
            UnityEngine.Debug.Log($"World Size: {world.size}");

            #region BallEntity初始化
            world.ball.speed.x = Number.ZERO;
            world.ball.speed.y = Define.BALL_MOVE_SPEED;
            world.ball.position = Define.BALL_INITIAL_POSITION;
            world.ball.radius = Define.BALL_SIZE;
            #endregion

            #region PlayerEntity初始化
            world.players = new PlayerEntity[] { new PlayerEntity(), new PlayerEntity() };
            for(var i = 0; i < world.players.Length; i++)
            {
                var player = world.players[i];
                player.position = Define.PLAYER_INITIAL_POSITION[i];
                player.size = new Vector2(Define.PLAYER_SIZE.x, Define.PLAYER_SIZE.y);
                player.speed = Define.PLAYER_MOVE_SPEED;                
            }
            #endregion

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
