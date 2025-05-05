using Sirenix.OdinInspector;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 原生代码的桥接器
    /// </summary>
    [InfoBox("原生代码和Native代码之间的桥接器。可以用来接收原生代码传来的消息。")]
    public class NativeBridge : ASingletonMonoBehaviour<NativeBridge>
    {      
        /// <summary>
        /// 接收来自原生层的消息
        /// </summary>
        /// <param name="json"></param>
        public void OnMessage(string json)
        {
            Debug.Log(LogColor.Zero1("收到Native的消息:\n{0}", json));
        }        
    }
}