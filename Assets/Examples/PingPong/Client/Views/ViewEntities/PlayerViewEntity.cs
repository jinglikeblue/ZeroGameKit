using UnityEngine;

namespace PingPong
{
    /// <summary>
    /// 玩家视图实体
    /// </summary>
    public class PlayerViewEntity : BaseViewEntity
    {
        public PlayerViewEntity(GameObject gameObject) : base(gameObject)
        {
        }

        public void Update(WorldEntity worldEntity, PlayerEntity playerEntity, InterpolationInfoVO interpolationInfo)
        {
            var pos = transform.localPosition;
            var targetPos = new Vector3(playerEntity.position.x.ToFloat(), 0, playerEntity.position.y.ToFloat());
            var lerpPos = Vector3.Lerp(transform.localPosition, targetPos, interpolationInfo.lerpValue);
            transform.localPosition = lerpPos;
        }
    }
}
