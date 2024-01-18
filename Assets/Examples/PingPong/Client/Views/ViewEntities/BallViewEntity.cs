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

        public void Update(WorldEntity worldEntity)
        {
            var ball = worldEntity.ball;
            transform.position = new Vector3(ball.position.x.ToFloat(), 0, ball.position.y.ToFloat());
        }
    }
}
