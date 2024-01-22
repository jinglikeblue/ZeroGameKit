using Jing.FixedPointNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
