using System.Collections;
using Example;
using UnityEngine;
using Zero;

namespace PingPong
{
    /// <summary>
    /// 网络更新
    /// </summary>
    public class NetUpdateCommand: BaseCommand
    {
        private Coroutine _coroutine;
        
        protected override void ExcuteProcess()
        {
            _coroutine = ILBridge.Ins.StartCoroutine(this, UpdateNetLoop());
        }
        
        protected override void TerminateProcess()
        {
            ILBridge.Ins.StopCoroutine(_coroutine);
        }
        
        private IEnumerator UpdateNetLoop()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                Global.Ins.netModule.host.Update();
                Global.Ins.netModule.client.Update();
            }
        }
    }
}