using Jing;
using Jing.FixedPointNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zero;

namespace PingPong
{
    /// <summary>
    /// 游戏的入口
    /// </summary>
    public class PingPongGame
    {
        GameObject _gameObject;
        Action<object> _bridge;
        public GameCore gameCore { get; private set; }

        Thread _logicThread;

        WorldViewEntity _worldView;

        public PingPongGame(GameObject gameObject, Action<object> bridge)
        {
            _gameObject = gameObject;
            _bridge = bridge;

            gameCore = new GameCore(new Number(33, 1000));
            _worldView = new WorldViewEntity(gameObject);

            var renderBridge = _gameObject.AddComponent<RenderBridgeComponent>();
            renderBridge.onRenderUpdate += RenderUpdate;
            renderBridge.onDestroy += Destroy;
        }

        /// <summary>
        /// 渲染更新
        /// </summary>
        private void RenderUpdate()
        {
            #region 更新渲染
            if (gameCore.FrameData != null)
            {
                _worldView.Update(gameCore.FrameData.world);
            }
            #endregion

            #region 渲染完成后，采集输入
            #endregion
        }

        public void Start()
        {
            //启动逻辑线程            
            _logicThread = new Thread(LogicUpdate);
            _logicThread.IsBackground = true;
            _logicThread.Name = "GameCoreThread";
            _logicThread.Start();
        }

        /// <summary>
        /// 逻辑线程更新
        /// </summary>
        private void LogicUpdate()
        {
            //GameCore刷新间隔
            var interval = gameCore.FrameInterval;

            var threadStartTime = TimeUtility.NowUtcMilliseconds;

            //游戏核心最后一次更新的时间
            var lastGameCoreUpdateTime = 0;

            //只要逻辑线程的引用还存在，则线程持续迭代
            while (_logicThread != null)
            {
                //距离线程启动经过了的时间
                var threadPastTime = TimeUtility.NowUtcMilliseconds - threadStartTime;
                //距离上次游戏核心更新，经过了的时间
                var pastTime = threadPastTime - lastGameCoreUpdateTime;
                //距离上次游戏核心更新，经过了的帧数
                var pastFrameCount = pastTime / interval;

                if (0 == pastFrameCount)
                {
                    //表示没有更新，这个时候让线程睡眠1毫秒，避免CPU占用100%
                    Thread.Sleep(1);
                }
                else
                {
                    if (pastFrameCount > 1)
                    {
                        //说明设备卡顿了，需要进行优化
                        Debug.LogWarning($"[LogicUpdate] 更新帧数");
                    }

                    for (var i = 0; i < pastFrameCount; i++)
                    {
                        //刷新游戏核心最后一次更新时间
                        lastGameCoreUpdateTime += interval;

                        //TODO 更新逻辑线程，这个时候需要传入输入数据
                        FrameInput input = FrameInput.Default;

                        PerformanceAnalysis.BeginAnalysis("GameCore_Update");
                        gameCore.Update(input);
                        PerformanceAnalysis.EndAnalysis("GameCore_Update");
                    }
                }
            }

            Debug.Log($"[LogicUpdate] GameCore线程结束");
        }

        public void Pause()
        {

        }

        public void Destroy()
        {
            var renderBridge = _gameObject.GetComponent<RenderBridgeComponent>();
            renderBridge.onRenderUpdate -= RenderUpdate;

            _logicThread = null;
        }
    }
}
