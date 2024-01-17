using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void Update(WorldEntity worldEntity, PlayerEntity playerEntity)
        {

        }
    }
}
