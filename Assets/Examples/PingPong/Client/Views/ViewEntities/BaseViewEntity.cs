using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PingPong
{
    /// <summary>
    /// 视图实例基类
    /// </summary>
    public class BaseViewEntity
    {
        /// <summary>
        /// 关联的GameObject对象
        /// </summary>
        public GameObject gameObject { get; private set; }

        /// <summary>
        /// 关联对象的Transform
        /// </summary>
        public Transform transform
        {
            get
            {
                return gameObject?.transform;
            }
        }

        public BaseViewEntity(GameObject gameObject)
        {
            Init(gameObject);
        }

        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
            OnInited();
        }

        protected virtual void OnInited()
        {

        }
    }
}
