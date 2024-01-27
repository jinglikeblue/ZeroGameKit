using Jing.FixedPointNumber;

namespace PingPong
{
    /// <summary>
    /// 球员辅助类
    /// </summary>
    public static class PlayerUtility
    {
        public static Rect CreatePlayerBody(PlayerEntity player)
        {
            var body = new Rect(Vector2.ZERO, player.size);
            body.center = player.position;
            return body;
        }
    }
}
