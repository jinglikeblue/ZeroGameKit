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
    /// 世界视图实体
    /// </summary>
    public class WorldViewEntity : BaseViewEntity
    {
        BallViewEntity ball;
        PlayerViewEntity[] players = new PlayerViewEntity[2];

        public WorldViewEntity(GameObject gameObject) : base(gameObject)
        {
            ball = new BallViewEntity(transform.Find("Objects/ViewEntities/Ball").gameObject);
            players[0] = new PlayerViewEntity(transform.Find("Objects/ViewEntities/Player0").gameObject);
            players[1] = new PlayerViewEntity(transform.Find("Objects/ViewEntities/Player1").gameObject);
        }

        public void Update(WorldEntity worldEntity, InterpolationInfoVO interpolationInfo)
        {            
            ball.Update(worldEntity, interpolationInfo);
            for(int i = 0; i < worldEntity.players.Length; i++)
            {
                players[i].Update(worldEntity, worldEntity.players[i], interpolationInfo);
            }
        }
    }
}
