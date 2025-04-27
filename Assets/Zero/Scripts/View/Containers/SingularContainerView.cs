namespace Zero
{
    /// <summary>
    /// 单一的视图层，该层中的视图，只能存在一个，视图之间的关系是切换
    /// </summary>
    public class SingularContainerView : BaseContainerView
    {
        /// <summary>
        /// 当前的视图
        /// </summary>
        public AView Current { get; private set; }

        protected override void BeforeShow()
        {
            Clear();
        }

        /// <summary>
        /// 切换视图
        /// </summary>
        /// <param name="view"></param>
        protected override void ShowView(AView view)
        {
            if (null != Current)
            {
                throw new System.Exception("框架逻辑代码有问题！这里Current应该在BeforeShow中已被设置为Null");
                //Current.onDestroyed -= OnViewDestroy;
                //Current.Destroy();
                //Current = null;
            }

            Current = view;
            view.onDestroyed += OnViewDestroy;
        }

        /// <summary>
        /// View对象销毁的回调
        /// </summary>
        /// <param name="view"></param>
        void OnViewDestroy(AView view)
        {
            if (Current == view)
            {
                Current.onDestroyed -= OnViewDestroy;
                Current = null;
            }
        }

        public override void Clear()
        {
            if (null != Current)
            {
                Current.onDestroyed -= OnViewDestroy;
                Current.Destroy();
                Current = null;
            }
        }
        
        /// <summary>
        /// 以指定类型来获取当前视图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>null表示当前视图不是T类型</returns>
        public T GetCurrent<T>() where T : AView
        {
            return Current as T;
        }
    }
}
