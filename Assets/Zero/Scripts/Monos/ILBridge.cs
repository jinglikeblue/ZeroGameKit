using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// IL代码执行桥接器。如果可以通过反射获取动态代码，则通过反射执行。否则采用ILRuntime框架执行。
    /// </summary>
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
        /// 当前工作的ILWorker类型
        /// </summary>
        public EILType ILWorkerType { get; private set; }

        /// <summary>
        /// 当ILRuntime模式时存在值
        /// </summary>
        public ILRuntime.Runtime.Enviorment.AppDomain ILRuntimeAppDomain { get; private set; }

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

            //如果是HybridCLR模式
            if (Runtime.Ins.ILType == EILType.HYBRID_CLR)
            {
                Debug.Log(LogColor.Zero1("外部程序集执行方式：[HYBRID_CLR]"));
                iLWorker = new HuaTuoILWorker(assembly);
                ILWorkerType = EILType.HYBRID_CLR;
                return;
            }

            if(Runtime.Ins.ILType == EILType.IL_RUNTIME)
            {
                if (null != assembly && Runtime.Ins.IsTryJitBeforeILRuntime)
                {
                    //可以用JIT方式执行
                    Debug.Log(LogColor.Zero1("外部程序集执行方式：[JIT]"));
                    //使用Assembly                
                    iLWorker = new AssemblyILWorker(assembly);
                    ILWorkerType = EILType.JIT;
                }
                else
                {
                    //如果JIT不行，则切换为ILRuntime模式
                    Debug.Log(LogColor.Zero1("外部程序集执行方式：[IL_RUNTIME]"));
                    //使用ILRuntime
                    var ilruntimeWorker = new ILRuntimeILWorker(dllBytes, pdbBytes, Runtime.Ins.IsDebugILRuntime);
                    iLWorker = ilruntimeWorker;
                    ILRuntimeAppDomain = ilruntimeWorker.appDomain;
                    ILWorkerType = EILType.IL_RUNTIME;
                }
            }
            else
            {
                throw new Exception("外部程序集执行出错！");
            }
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
                cp.onDestroy += (proxy) =>
                {
                    _routineDic.Remove(proxy.bindingObj);
                };
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