using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Example;
using UnityEngine;
using Zero;

namespace PingPong
{
    /// <summary>
    /// 网络更新
    /// </summary>
    public class NetUpdateCommand : BaseCommand
    {
        private bool _isLoopActively;

        protected override void ExecuteProcess()
        {
            UpdateNetLoop();
        }

        protected override void TerminateProcess()
        {
            _isLoopActively = false;
        }

        private async void UpdateNetLoop()
        {
            try
            {
                _isLoopActively = true;
                while (_isLoopActively)
                {
                    await UniTask.NextFrame();
                    Global.Ins.netModule.host.Update();
                    Global.Ins.netModule.client.Update();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}