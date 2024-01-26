using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zero;

namespace ZeroHot
{
    /// <summary>
    /// 视图工厂
    /// </summary>
    public sealed class ViewFactory
    {
        static readonly Type _viewRegisterAttr = typeof(ViewRegisterAttribute);

        /// <summary>
        /// [视图名称] => [AssestBundle]
        /// </summary>
        static Dictionary<string, string> _viewAssetBundleSearchDic;
        static ViewFactory()
        {
            CreateViewAssetBundleSearchDictionary();
        }

        /// <summary>
        /// 创建视图的AssetBundle查找表（多个视图同名的话，则表中没有该视图的记录，因为不精确）
        /// </summary>
        public static void CreateViewAssetBundleSearchDictionary()
        {
            if(_viewAssetBundleSearchDic != null)
            {
                return;
            }

            _viewAssetBundleSearchDic = AB.CreateViewAssetBundleSearchDictionary();            
            var s = LitJson.JsonMapper.ToJson(_viewAssetBundleSearchDic);
            Debug.Log(s);
        }        

        /// <summary>
        /// 查找Type对应的AB信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="abName"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        static bool FindAssetBundleInfo(Type type, out string abName, out string viewName)
        {            
            var attrs = type.GetCustomAttributes(_viewRegisterAttr, false);
            if(attrs.Length == 0)
            {
                //从自动表查找
                if (false == _viewAssetBundleSearchDic.ContainsKey(type.Name))
                {
                    abName = null;
                    viewName = null;
                    return false;
                }
                
                viewName = type.Name;
                abName = _viewAssetBundleSearchDic[viewName];
            }
            else
            {
                var attr = attrs[0] as ViewRegisterAttribute;
                ResMgr.Ins.SeparateAssetPath(attr.prefabPath, out abName, out viewName);

                //abName += ".ab";
                //viewName = Path.GetFileNameWithoutExtension(viewName);                
            }

            return true;
        }

        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static AView Create(Type type, GameObject prefab, Transform parent, object data = null)
        {
            GameObject go = GameObject.Instantiate(prefab, parent);
            go.name = prefab.name;

            AView view = Activator.CreateInstance(type) as AView;
            view.SetGameObject(go, data);
            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Create<T>(GameObject prefab, Transform parent, object data = null) where T : AView
        {
            AView view = Create(typeof(T), prefab, parent, data);
            return view as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="abName"></param>
        /// <param name="viewName"></param>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static AView Create(Type type, string abName, string viewName, Transform parent, object data = null)
        {            
            GameObject prefab = ResMgr.Ins.Load<GameObject>(abName, viewName);            
            return Create(type, prefab, parent, data);
        }

        public static T Create<T>(string abName, string viewName, Transform parent, object data = null) where T : AView
        {
            GameObject prefab = ResMgr.Ins.Load<GameObject>(abName, viewName);            
            return Create<T>(prefab, parent, data);
        }

        public static AView Create(Type type, Transform parent, object data = null)
        {
            string abName, viewName;                                
            if (FindAssetBundleInfo(type, out abName, out viewName))
            {                
                return Create(type, abName, viewName, parent, data);
            }
            else
            {
                Debug.LogErrorFormat("AView类[{0}]并没有适用的视图。请检查是否配置或生成了资源名清单", type.FullName);
            }
            return null;
        }

        public static T Create<T>(Transform parent, object data = null) where T : AView
        {
            Type type = typeof(T);
            return Create(type, parent, data) as T;
        }    

        public static void CreateAsync(Type type, string abName, string viewName, Transform parent, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null)
        {
            new ViewAsyncCreater<AView>(type, abName, viewName).Create(parent,data,onCreated,onProgress, onLoaded);
        }

        public static void CreateAsync<T>(string abName, string viewName, Transform parent, object data = null, Action<T> onCreated = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null) where T : AView
        {
            new ViewAsyncCreater<T>(typeof(T), abName, viewName).Create(parent, data, onCreated, onProgress, onLoaded);
        }

        public static void CreateAsync(Type type, Transform parent, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null)
        {
            string abName, viewName;
            if (FindAssetBundleInfo(type, out abName, out viewName))
            {
                new ViewAsyncCreater<AView>(type, abName, viewName).Create(parent, data, onCreated, onProgress, onLoaded);
            }
            else
            {
                Debug.LogErrorFormat("AView类[{0}]并没有适用的视图", type.FullName);
            }
        }

        public static void CreateAsync<T>(Transform parent, object data = null, Action<T> onCreated = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null) where T : AView
        {
            Type type = typeof(T);
            string abName, viewName;
            if (FindAssetBundleInfo(type, out abName, out viewName))
            {
                new ViewAsyncCreater<T>(type, abName, viewName).Create(parent, data, onCreated, onProgress, onLoaded);
            }
            else
            {
                Debug.LogErrorFormat("AView类[{0}]并没有适用的视图", type.FullName);
            }
        }
    }
}
