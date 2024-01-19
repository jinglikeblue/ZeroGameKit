using Jing.FixedPointNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PingPong
{
    /// <summary>
    /// 球体视图实体
    /// </summary>
    public class BallViewEntity : BaseViewEntity
    {
        public BallViewEntity(GameObject gameObject) : base(gameObject)
        {
        }

        protected override void OnInited()
        {
            base.OnInited();
        }

        public void Update(WorldEntity worldEntity, Number deltaTime)
        {
            var ball = worldEntity.ball;
            var newPos = MoveSystem.CalculateBallPosition(ball, deltaTime);
            var pos = transform.localPosition;
            pos.x = (float)System.Math.Round(newPos.x.ToDouble(), 2);
            pos.z = (float)System.Math.Round(newPos.y.ToDouble(), 2);
            transform.localPosition = pos;                                                 
        }
    }
}
