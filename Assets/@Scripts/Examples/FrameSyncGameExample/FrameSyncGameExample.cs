using Example.FrameSyncGame;
using Jing.FixedPointNumber;
using PingPong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zero;
using ZeroGameKit;

namespace Example
{
    class FrameSyncGameExample
    {
        static public void Start()
        {
            var shader = Shader.Find("Zero/UI/TextOutline");
            var objs = ResMgr.Ins.LoadAll("appends/shaders");
            foreach(var obj in objs)
            {
                if(obj is ShaderVariantCollection)
                {
                    ((ShaderVariantCollection)obj).WarmUp();
                }
                else if(obj is Shader)
                {
                    // 通过在Material中使用Shader来预加载
                    Material material = new Material((Shader)obj);
                    Resources.UnloadAsset(material);
                }
                Debug.Log(obj.GetType().FullName);
            }
            UIPanelMgr.Ins.Switch<PingPongMenuPanel>();
        }
    }
}
