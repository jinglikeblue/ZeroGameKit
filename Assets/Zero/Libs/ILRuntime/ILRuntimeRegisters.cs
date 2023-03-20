using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// ILRuntime的泛型适配器、重定向等功能注册类。
    /// 在使用泛型委托时，ILRuntime可能会因为发现热更代码向主工程注册的委托因为没有适配器而报错。
    /// 报错代码类似：KeyNotFoundException: Cannot find Delegate Adapter for:[XXX], Please add following code:
    /// 这时只需要将报错内容提示的代码(following code下面一行)粘贴到该类的Register方法中即可。
    /// </summary>
    public sealed class ILRuntimeRegisters
    {
        public ILRuntime.Runtime.Enviorment.AppDomain appdomain { get; }

        public ILRuntimeRegisters(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            this.appdomain = appdomain;
        }

        public void Register()
        {
            RegisterAdaptor();
            RegisterCLRRedirection();
            RegisterFunctionDelegate();
            RegisterDelegateConvertor();            
        }

        /// <summary>
        /// 注册适配器
        /// </summary>
        void RegisterAdaptor()
        {
            #region Zero框架使用
            //使用Couroutine时，C#编译器会自动生成一个实现了IEnumerator，IEnumerator<object>，IDisposable接口的类，因为这是跨域继承，所以需要写CrossBindAdapter
            appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            appdomain.RegisterCrossBindingAdaptor(new Adapt_IMessage());
            //appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder()); //能提高性能，但可能导致BUG
            //appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder()); //能提高性能，但可能导致BUG 
            //appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder()); //能提高性能，但可能导致BUG
            #endregion
        }

        /// <summary>
        /// 注册重定向
        /// </summary>
        void RegisterCLRRedirection()
        {
            #region Zero框架使用
            //注册LitJson
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);
            #endregion
            
        }

        /// <summary>
        /// 注册方法委托
        /// </summary>
        void RegisterFunctionDelegate()
        {
            #region Zero框架使用
            appdomain.DelegateManager.RegisterMethodDelegate<float>();
            appdomain.DelegateManager.RegisterMethodDelegate<PointerEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<AxisEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Collider2D>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.Net.DownloadProgressChangedEventArgs>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.ComponentModel.AsyncCompletedEventArgs>();
            appdomain.DelegateManager.RegisterMethodDelegate<Zero.Timer>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Single, System.Int64>();            appdomain.DelegateManager.RegisterMethodDelegate<System.String>();            appdomain.DelegateManager.RegisterFunctionDelegate<global::Adapt_IMessage.Adaptor>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object[]>();
            appdomain.DelegateManager.RegisterMethodDelegate<Zero.HttpDownloader, System.Collections.Generic.Dictionary<System.String, System.String>>();            appdomain.DelegateManager.RegisterMethodDelegate<Zero.HttpDownloader, System.Single, System.Int32>();            appdomain.DelegateManager.RegisterMethodDelegate<Zero.HttpDownloader>();            appdomain.DelegateManager.RegisterMethodDelegate<Zero.GroupHttpDownloader>();            appdomain.DelegateManager.RegisterMethodDelegate<Zero.GroupHttpDownloader, System.Single, System.Int32>();            appdomain.DelegateManager.RegisterMethodDelegate<Zero.GroupHttpDownloader, Zero.GroupHttpDownloader.TaskInfo>();            appdomain.DelegateManager.RegisterMethodDelegate<Zero.GroupHttpDownloader, Zero.GroupHttpDownloader.TaskInfo, System.Collections.Generic.Dictionary<System.String, System.String>>();
            #endregion

            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector2>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Sprite>();            appdomain.DelegateManager.RegisterMethodDelegate<One.IChannel>();            appdomain.DelegateManager.RegisterMethodDelegate<Jing.TurbochargedScrollList.ScrollListItem, System.Object, System.Boolean>();            appdomain.DelegateManager.RegisterMethodDelegate<One.TcpClient>();            appdomain.DelegateManager.RegisterMethodDelegate<One.UdpServer, System.Net.EndPoint, System.Byte[]>();            appdomain.DelegateManager.RegisterMethodDelegate<One.WebSocketClient>();            appdomain.DelegateManager.RegisterMethodDelegate<One.TcpClient, System.Byte[]>();            appdomain.DelegateManager.RegisterMethodDelegate<One.UdpClient, System.Byte[]>();            appdomain.DelegateManager.RegisterMethodDelegate<One.WebSocketClient, System.Byte[]>();            appdomain.DelegateManager.RegisterMethodDelegate<One.IChannel, System.Byte[]>();            appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Object, System.Boolean>();            appdomain.DelegateManager.RegisterMethodDelegate<Zero.BaseUpdater>();            appdomain.DelegateManager.RegisterMethodDelegate<System.Int64, System.Int64>();
        }

        /// <summary>
        /// 注册方法转换
        /// </summary>
        void RegisterDelegateConvertor()
        {
            #region Zero框架使用
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Net.DownloadProgressChangedEventHandler>((act) =>
            {
                return new System.Net.DownloadProgressChangedEventHandler((sender, e) =>
                {
                    ((Action<System.Object, System.Net.DownloadProgressChangedEventArgs>)act)(sender, e);
                });
            });
            
            appdomain.DelegateManager.RegisterDelegateConvertor<System.ComponentModel.AsyncCompletedEventHandler>((act) =>
            {
                return new System.ComponentModel.AsyncCompletedEventHandler((sender, e) =>
                {
                    ((Action<System.Object, System.ComponentModel.AsyncCompletedEventArgs>)act)(sender, e);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.ThreadStart>((act) =>
            {
                return new System.Threading.ThreadStart(() =>
                {
                    ((Action)act)();
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((Action)act)();
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<Zero.HttpDownloader.ReceivedResponseHeadersEvent>((act) =>
            {
                return new Zero.HttpDownloader.ReceivedResponseHeadersEvent((downloader, responseHeaders) =>
                {
                    ((Action<Zero.HttpDownloader, System.Collections.Generic.Dictionary<System.String, System.String>>)act)(downloader, responseHeaders);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Zero.HttpDownloader.ProgressEvent>((act) =>
            {
                return new Zero.HttpDownloader.ProgressEvent((downloader, progress, contentLength) =>
                {
                    ((Action<Zero.HttpDownloader, System.Single, System.Int32>)act)(downloader, progress, contentLength);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Zero.HttpDownloader.CompletedEvent>((act) =>
            {
                return new Zero.HttpDownloader.CompletedEvent((downloader) =>
                {
                    ((Action<Zero.HttpDownloader>)act)(downloader);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Zero.GroupHttpDownloader.CompletedEvent>((act) =>
            {
                return new Zero.GroupHttpDownloader.CompletedEvent((groupDownloader) =>
                {
                    ((Action<Zero.GroupHttpDownloader>)act)(groupDownloader);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Zero.GroupHttpDownloader.ProgressEvent>((act) =>
            {
                return new Zero.GroupHttpDownloader.ProgressEvent((groupDownloader, progress, contentLength) =>
                {
                    ((Action<Zero.GroupHttpDownloader, System.Single, System.Int32>)act)(groupDownloader, progress, contentLength);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Zero.GroupHttpDownloader.TaskCompletedEvent>((act) =>
            {
                return new Zero.GroupHttpDownloader.TaskCompletedEvent((groupDownloader, taskInfo) =>
                {
                    ((Action<Zero.GroupHttpDownloader, Zero.GroupHttpDownloader.TaskInfo>)act)(groupDownloader, taskInfo);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Zero.GroupHttpDownloader.TaskStartedEvent>((act) =>
            {
                return new Zero.GroupHttpDownloader.TaskStartedEvent((groupDownloader, taskInfo, responseHeaders) =>
                {
                    ((Action<Zero.GroupHttpDownloader, Zero.GroupHttpDownloader.TaskInfo, System.Collections.Generic.Dictionary<System.String, System.String>>)act)(groupDownloader, taskInfo, responseHeaders);
                });
            });
            #endregion

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Boolean>((arg0) =>
                {
                    ((Action<System.Boolean>)act)(arg0);
                });
            });            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Single>((arg0) =>
                {
                    ((Action<System.Single>)act)(arg0);
                });
            });            appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
            {
                return new DG.Tweening.TweenCallback(() =>
                {
                    ((Action)act)();
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<One.ReceiveDataEvent>((act) =>
            {
                return new One.ReceiveDataEvent((sender, data) =>
                {
                    ((Action<One.IChannel, System.Byte[]>)act)(sender, data);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Jing.TurbochargedScrollList.RenderItemDelegate>((act) =>
            {
                return new Jing.TurbochargedScrollList.RenderItemDelegate((item, data, isFresh) =>
                {
                    ((Action<Jing.TurbochargedScrollList.ScrollListItem, System.Object, System.Boolean>)act)(item, data, isFresh);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Zero.BaseUpdater.UpdateProgress>((act) =>
            {
                return new Zero.BaseUpdater.UpdateProgress((loadedSize, totalSize) =>
                {
                    ((Action<System.Int64, System.Int64>)act)(loadedSize, totalSize);
                });
            });
        }
    }
}