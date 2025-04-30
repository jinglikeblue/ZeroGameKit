using System;
using Cysharp.Threading.Tasks;
using Zero;

namespace ZeroGameKit
{
    public class PanelContainerView : SingularContainerView
    {
        /// <summary>
        /// 切换UIPanel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public T Switch<T>(object data = null) where T : AView
        {
            Clear();
            //生成新的界面
            var view = Show<T>(data);
            return view;
        }

        /// <summary>
        /// 切换UIPanel
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public AView Switch(Type type, object data = null)
        {
            Clear();
            //生成新的界面
            var view = Show(type, data);
            return view;
        }

        /// <summary>
        /// 异步切换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public async UniTask<T> SwitchAsync<T>(object data = null, Action<T> onCreated = null, Action<float> onProgress = null) where T : AView
        {
            var view = await SwitchAsync(typeof(T), data, null, onProgress) as T;
            onCreated?.Invoke(view);
            return view;
        }

        /// <summary>
        /// 异步切换
        /// </summary>
        /// <param name="viewType">AView的子类</param>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public async UniTask<AView> SwitchAsync(Type viewType, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null)
        {
            var view = await ShowAsync(viewType, data, onCreated, onProgress);
            return view;
        }
    }
}