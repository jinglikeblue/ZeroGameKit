using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Zero
{
    [InfoBox("Native代码和Hot代码之间的桥接器。可以通过反射调用Hot代码。")]
    public class ILBridge : ASingletonMonoBehaviour<ILBridge>
    {
        /// <summary>
        /// Update事件委托
        /// </summary>
        public event Action onUpdate;

        /// <summary>
        /// OnGUI事件委托
        /// </summary>
        public event Action onGUI;

        /// <summary>
        /// OnFixedUpdate事件委托
        /// </summary>
        public event Action onFixedUpdate;

        /// <summary>
        /// OnLateUpdate事件委托
        /// </summary>
        public event Action onLateUpdate;

        /// <summary>
        /// 客户端焦点事件
        /// </summary>
        public event Action<bool> onApplicationFocus;

        /// <summary>
        /// 客户端暂停事件
        /// </summary>
        public event Action<bool> onApplicationPause;

        /// <summary>
        /// 程序退出
        /// </summary>
        public event Action onApplicationQuit;

        /// <summary>
        /// IL代码执行的工作器
        /// </summary>
        BaseILWorker iLWorker;

        /// <summary>
        /// 获取代码域中的类型清单
        /// </summary>
        /// <param name="whereFunc">可选参数，委托通过参数Type判断是否需要加入清单中，返回true则表示需要</param>
        /// <returns></returns>
        public Type[] GetTypes(Func<Type, bool> whereFunc = null)
        {
            return iLWorker.GetTypes(whereFunc);
        }

        public void Startup()
        {
            //使用Assembly
            iLWorker = new AssemblyILWorker(this.GetType().Assembly);
        }

        /// <summary>
        /// 启动热更代码执行
        /// </summary>
        /// <param name="dllBytes">dll文件二进制数据</param>
        /// <param name="pdbBytes">pdb文件二进制数据，可能为null</param>         
        public void Startup(byte[] dllBytes, byte[] pdbBytes)
        {
            Assembly assembly = AssemblyILWorker.LoadAssembly(dllBytes, pdbBytes);

            if (null == assembly)
            {
                throw new Exception("外部程序集加载失败！");
            }
            
            // Debug.Log(LogColor.Zero1("外部程序集执行方式：[HYBRID_CLR]"));
            iLWorker = new HybridCLRWorker(assembly);

            // if (TypeUtility.FindAssembly("HybridCLR.Runtime") != null)
            // {
            //
            // }
            // else
            // {
            //     Debug.Log(LogColor.Zero1("外部程序集执行方式：[JIT]"));
            //     iLWorker = new AssemblyILWorker(assembly);
            // }
        }

        public void Invoke(string clsName, string methodName)
        {
            iLWorker.Invoke(clsName, methodName);
        }

        private void OnGUI()
        {
            onGUI?.Invoke();
        }

        void Update()
        {
            onUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            onFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            onLateUpdate?.Invoke();
        }

        private void OnApplicationFocus(bool focus)
        {
            onApplicationFocus?.Invoke(focus);
        }

        private void OnApplicationPause(bool pause)
        {
            onApplicationPause?.Invoke(pause);
        }

        private void OnApplicationQuit()
        {
            onApplicationQuit?.Invoke();
        }

        #region 协程代理

        Dictionary<object, CoroutineProxy> _routineDic = new Dictionary<object, CoroutineProxy>();

        CoroutineProxy GetCoroutineProxy(object target, bool isAutoCreate)
        {
            CoroutineProxy cp;
            _routineDic.TryGetValue(target, out cp);

            if (null == cp && isAutoCreate)
            {
                GameObject go = new GameObject("CoroutineProxy_" + target.GetHashCode());
                go.transform.SetParent(transform);
                cp = go.AddComponent<CoroutineProxy>();
                cp.bindingObj = target;
                cp.onDestroy += (proxy) => { _routineDic.Remove(proxy.bindingObj); };
                _routineDic[target] = cp;
            }

            return cp;
        }

        public Coroutine StartCoroutine(object target, IEnumerator coroutine)
        {
            var cp = GetCoroutineProxy(target, true);
            return cp.StartTrackedCoroutine(coroutine);
        }

        public void StopCoroutine(object target, IEnumerator routine)
        {
            var cp = GetCoroutineProxy(target, false);
            cp?.StopTrackedCoroutine(routine);
        }

        public void StopCoroutine(object target, Coroutine routine)
        {
            var cp = GetCoroutineProxy(target, false);
            cp?.StopTrackedCoroutine(routine);
        }

        public void StopAllCoroutines(object target)
        {
            var cp = GetCoroutineProxy(target, false);
            if (null != cp)
            {
                cp.StopAllTrackedCoroutines();
            }
        }

        #endregion
    }
}