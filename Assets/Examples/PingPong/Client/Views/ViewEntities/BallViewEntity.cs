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

        public void Update(WorldEntity worldEntity, InterpolationInfoVO interpolationInfo)
        {
            var ball = worldEntity.ball;

            var targetPos = new Vector3(ball.position.x.ToFloat(), 0, ball.position.y.ToFloat());
            var lerpPos = Vector3.Lerp(transform.localPosition, targetPos, interpolationInfo.lerpValue);
            transform.localPosition = lerpPos;                                                 
        }
    }
}
