using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;

namespace ZeroEditor
{
    /// <summary>
    /// 编辑器协程类
    /// </summary>
    public static class EditorCoroutines
    {
        public const string DefaultGroup = "Default";

        /// <summary>
        /// 分组协程列表
        /// </summary>
        private static readonly Dictionary<string, List<EditorCoroutine>> GroupCoroutineListDict = new Dictionary<string, List<EditorCoroutine>>();

        /// <summary>
        /// 启动一个编辑器协程
        /// </summary>
        /// <param name="routine">协程方法</param>
        /// <param name="owner">协程的持有者，如果传入EditorWindow实例，那么实例销毁时，协程也会终止</param>
        /// <param name="groupName">协程分组名称</param>
        /// <returns></returns>
        public static EditorCoroutine Start(IEnumerator routine, UnityEngine.Object owner = null, string groupName = DefaultGroup)
        {
            var coroutine = owner ? EditorCoroutineUtility.StartCoroutine(routine, owner) : EditorCoroutineUtility.StartCoroutineOwnerless(routine);

            GetList(groupName).Add(coroutine);

            return coroutine;
        }

        /// <summary>
        /// 终止一个编辑器协程
        /// </summary>
        /// <param name="coroutine"></param>
        public static void Stop(EditorCoroutine coroutine)
        {
            foreach (var list in GroupCoroutineListDict.Values)
            {
                list.Remove(coroutine);
            }
            EditorCoroutineUtility.StopCoroutine(coroutine);
        }

        /// <summary>
        /// 终止一个编辑器协程组
        /// </summary>
        /// <param name="groupName"></param>
        public static void Stop(string groupName)
        {
            var list = GetList(groupName);
            foreach (var coroutine in list)
            {
                EditorCoroutineUtility.StopCoroutine(coroutine);
            }
            list.Clear();
        }

        /// <summary>
        /// 终止所有的编辑器协程
        /// </summary>
        public static void StopAll()
        {
            foreach (var groupName in GroupCoroutineListDict.Keys)
            {
                Stop(groupName);
            }
        }

        private static List<EditorCoroutine> GetList(string groupName)
        {
            if (null == groupName)
            {
                groupName = DefaultGroup;
            }

            if (!GroupCoroutineListDict.TryGetValue(groupName, out var list))
            {
                list = new List<EditorCoroutine>();
                GroupCoroutineListDict.Add(groupName, list);
            }

            return list;
        }
    }
}