﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PingPong
{
    /// <summary>
    /// 渲染回调桥接组件
    /// </summary>
    public class RenderBridgeComponent : MonoBehaviour
    {
        /// <summary>
        /// 渲染更新
        /// </summary>
        public event Action onRenderUpdate;

        /// <summary>
        /// 被销毁
        /// </summary>
        public event Action onDestroy;

        public event Action<bool> onAICoreUpdateEnableChanged;
        public event Action<bool> onGameCoreUpdateEnableChanged;


        bool _lastGameCoreUpdateEnable;

        [Header("是否允许GameCore更新")]
        public bool isGameCoreUpdateEnable = true;

        bool _lastAICoreUpdateEnable;

        [Header("是否允许AI更新")]
        public bool isAICoreUpdateEnable = true;

        private void Awake()
        {
            _lastAICoreUpdateEnable = isAICoreUpdateEnable;
            _lastGameCoreUpdateEnable = isGameCoreUpdateEnable;
        }

        private void Update()
        {
            onRenderUpdate?.Invoke();
            if(_lastAICoreUpdateEnable != isAICoreUpdateEnable)
            {
                _lastAICoreUpdateEnable = isAICoreUpdateEnable;
                onAICoreUpdateEnableChanged?.Invoke(_lastAICoreUpdateEnable);
            }

            if (_lastGameCoreUpdateEnable != isGameCoreUpdateEnable)
            {
                _lastGameCoreUpdateEnable = isGameCoreUpdateEnable;
                onGameCoreUpdateEnableChanged?.Invoke(_lastGameCoreUpdateEnable);
            }
        }

        private void OnDestroy()
        {
            onDestroy?.Invoke();
        }
    }
}
