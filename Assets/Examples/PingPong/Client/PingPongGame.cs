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
        /// <summary>
        /// GameCore每秒刷新率
        /// </summary>
        const int GAME_CORE_FPS = 30;

        GameObject _gameObject;
        Action<object> _bridge;
        public GameCore gameCore { get; private set; }

        Thread _logicThread;

        WorldViewEntity _worldView;

        /// <summary>
        /// 计时器
        /// </summary>
        Chronograph _chronographer;

        /// <summary>
        /// 输入控制器
        /// </summary>
        InputController _inputController;

        /// <summary>
        /// 插值信息数据
        /// </summary>
        InterpolationInfoVO _interpolationInfo;

        /// <summary>
        /// AI核心
        /// </summary>
        AICore _aiCore;

        /// <summary>
        /// AI线程
        /// </summary>
        Thread _aiThread;


        public PingPongGame(GameObject gameObject, Action<object> bridge)
        {
            _gameObject = gameObject;
            _bridge = bridge;

            _chronographer = new Chronograph();
            _inputController = new InputController();

            gameCore = new GameCore(Number.ONE / GAME_CORE_FPS);
            _worldView = new WorldViewEntity(gameObject);
            _interpolationInfo = new InterpolationInfoVO();

            var renderBridge = _gameObject.AddComponent<RenderBridgeComponent>();
            renderBridge.onRenderUpdate += RenderUpdate;
            renderBridge.onDestroy += Destroy;

            _aiCore = new AICore();
            _aiCore.Init(new int[] { 1 });
        }

        /// <summary>
        /// 渲染更新
        /// </summary>
        private void RenderUpdate()
        {
            UpdateInterpolationInfo();

            GUIDebugInfo.SetInfo("DeltaTime", _interpolationInfo);

            #region 更新渲染
            if (gameCore.FrameData != null)
            {
                _worldView.Update(gameCore.FrameData.world, _interpolationInfo);
            }
            #endregion

            #region 渲染完成后，采集输入
            _inputController.CollectInput();
            #endregion

            #region 检查GameCore更新控制
            var renderBridge = _gameObject.GetComponent<RenderBridgeComponent>();
            if (renderBridge.isGameCoreUpdateEnable && false == _chronographer.IsRunning)
            {
                _chronographer.Start();
            }
            else if (false == renderBridge.isGameCoreUpdateEnable && true == _chronographer.IsRunning)
            {
                _chronographer.Pause();
            }
            #endregion
        }

        public void Start()
        {
            //启动逻辑线程            
            _logicThread = new Thread(LogicUpdate);
            _logicThread.IsBackground = true;
            _logicThread.Name = "GameCoreThread";
            _logicThread.Start();

            //AI线程
            _aiThread = new Thread(AIUpdate);
            _aiThread.IsBackground = true;
            _aiThread.Name = "AIThread";
            _aiThread.Start();
        }

        /// <summary>
        /// 距离上次游戏核心更新，经过了的时间
        /// </summary>
        /// <returns></returns>
        Number getPastTime()
        {
            var chronographerElapsedSeconds = new Number((int)_chronographer.ElapsedMilliseconds, 1000);
            var pastTime = chronographerElapsedSeconds - gameCore.FrameData.elapsedTime;
            return pastTime;
        }

        /// <summary>
        /// 创建插值信息数据
        /// </summary>
        /// <returns></returns>
        void UpdateInterpolationInfo()
        {
            var deltaSeconds = getPastTime();
            _interpolationInfo.deltaMS = (deltaSeconds * 1000).ToInt();
            _interpolationInfo.lerpValue = (deltaSeconds / gameCore.FrameInterval).ToFloat();
        }

        void AIUpdate()
        {
            while (_aiThread != null)
            {
                PerformanceAnalysis.BeginAnalysis("AICore_Update");
                //更新AI核心
                var isUpdated = _aiCore.Update(gameCore);
                PerformanceAnalysis.EndAnalysis("AICore_Update");

                if (false == isUpdated)
                {
                    Thread.Sleep(1);
                    continue;
                }

                //从AI核心中提取操作数据
                var agents = _aiCore.GetAgents();
                foreach (var agent in agents)
                {                    
                    _inputController.CollectAIBehavior(agent.PlayerIndex, agent.GetInput());
                }
            }
        }

        /// <summary>
        /// 逻辑线程更新
        /// </summary>
        void LogicUpdate()
        {
            _chronographer.Start();

            //只要逻辑线程的引用还存在，则线程持续迭代
            while (_logicThread != null)
            {
                if (gameCore.FrameData.world.state == EWorldState.END)
                {
                    //游戏结束了
                    break;
                }
                //var chronographerElapsedSeconds = new Number((int)_chronographer.ElapsedMilliseconds, 1000);
                //距离上次游戏核心更新，经过了的时间
                var pastTime = getPastTime();
                //距离上次游戏核心更新，经过了的帧数
                var pastFrameCount = (pastTime / gameCore.FrameInterval).ToInt();

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
                        //TODO 更新逻辑线程，这个时候需要传入输入数据
                        FrameInput input = FrameInput.Default;
                        var playerInput = _inputController.ExtractInput();
                        //if (playerInput.moveDir != EMoveDir.NONE)
                        //{
                        //    Debug.Log($"移动方向:{playerInput.moveDir}");
                        //}
                        input.playerInputs = playerInput;

                        PerformanceAnalysis.BeginAnalysis("GameCore_Update");
                        gameCore.Update(input);
                        PerformanceAnalysis.EndAnalysis("GameCore_Update");
                    }
                }
            }

            _chronographer.Stop();
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
