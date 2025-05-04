using System;
using Cysharp.Threading.Tasks;

namespace Zero
{
    /// <summary>
    /// 基础容器类
    /// </summary>
    public abstract class BaseContainerView : AView
    {
        /// <summary>
        /// 显示视图(使用该方式显示视图，请先在ViewFactory中注册AViewType)
        /// </summary>
        /// <typeparam name="AViewType"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public AViewType Show<AViewType>(object data = null) where AViewType : AView
        {
            BeforeShow();
            var view = ViewFactory.Create<AViewType>(transform, data);
            ShowView(view);
            return view;
        }

        /// <summary>
        /// 显示视图(使用该方式显示视图，请先在ViewFactory中注册type)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public AView Show(Type type, object data = null)
        {
            BeforeShow();
            var view = ViewFactory.Create(type, transform, data);
            ShowView(view);
            return view;
        }

        /// <summary>
        /// 异步创建视图
        /// </summary>
        /// <param name="viewType">AView的子类</param>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">视图创建完成的回调</param>
        /// <param name="onResProgress">视图资源的加载进度回调</param>
        /// <param name="onResLoaded">视图资源加载完成的回调(会在onCreated之前触发)</param>
        /// <returns></returns>
        public async UniTask<AView> ShowAsync(Type viewType, object data = null, Action<AView> onCreated = null, Action<float> onResProgress = null, Action<UnityEngine.Object> onResLoaded = null)
        {
            var view = await ViewFactory.CreateAsync(viewType, transform, data, onCreated, onResProgress, (resObj) =>
            {
                onResLoaded?.Invoke(resObj);
                BeforeShow();
            });
            ShowView(view);
            return view;
        }

        /// <summary>
        /// 异步显示视图(使用该方式显示视图，请先在ViewFactory中注册AViewType)
        /// </summary>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法，会传回显示的视图以及token对象</param>
        /// <param name="onResProgress">视图资源的加载进度回调</param>
        /// <param name="onResLoaded">视图资源加载完成的回调(会在onCreated之前触发)</param>
        public async UniTask<T> ShowAsync<T>(object data = null, Action<T> onCreated = null, Action<float> onResProgress = null, Action<UnityEngine.Object> onResLoaded = null) where T : AView
        {
            T view = await ShowAsync(typeof(T), data, null, onResProgress, onResLoaded) as T;
            onCreated?.Invoke(view);
            return view;
        }

        /// <summary>
        /// 当资源准备好准备开始生成视图对象前被调用
        /// </summary>
        /// <param name="view"></param>
        protected abstract void BeforeShow();

        /// <summary>
        /// 显示视图
        /// </summary>
        /// <param name="view"></param>
        protected abstract void ShowView(AView view);

        /// <summary>
        /// 清理视图
        /// </summary>
        public abstract void Clear();
    }
}