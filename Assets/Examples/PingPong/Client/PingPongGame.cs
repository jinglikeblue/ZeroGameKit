using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PingPong
{
    /// <summary>
    /// 游戏的入口
    /// </summary>
    public class PingPongGame
    {
        GameObject _gameObject;
        Action<object> _bridge;
        public PingPongGame(GameObject gameObject, Action<object> bridge)
        {
            _gameObject = gameObject;
            _bridge = bridge;
        }

        public void Start()
        {

        }
    }
}
