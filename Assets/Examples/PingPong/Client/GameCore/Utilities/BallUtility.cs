using Jing.FixedPointNumber;

namespace PingPong
{
    public static class BallUtility
    {
        public static Round CreateBallBody(BallEntity ball)
        {
            var body = new Round(ball.position.x, ball.position.y, ball.radius);
            return body;
        }
    }
}
