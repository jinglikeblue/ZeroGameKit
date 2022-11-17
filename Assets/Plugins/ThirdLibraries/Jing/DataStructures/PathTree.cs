using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jing
{
    public class PathTree : PathTree<object>
    {

    }

    /// <summary>
    /// 树状查询图，可以通过路径节点，一步一步找到对应的节点，获取或存储数据
    /// </summary>
    public class PathTree<T>
    {
        /// <summary>
        /// 节点的名称
        /// </summary>
        public string Name { get; private set; } = null;

        /// <summary>
        /// 父节点，如果为null表示该节点为根节点
        /// </summary>
        public PathTree<T> Parent { get; private set; } = null;

        /// <summary>
        /// 是否是根节点
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return Parent == null ? true : false;
            }
        }

        /// <summary>
        /// 子节点的表
        /// </summary>
        Dictionary<string, PathTree<T>> _childTreeMap = new Dictionary<string, PathTree<T>>();

        /// <summary>
        /// 子节点数量
        /// </summary>
        public int ChildrenCount
        {
            get
            {
                return _childTreeMap.Count;
            }
        }

        /// <summary>
        /// 子节点的名称
        /// </summary>
        public string[] GetChildrenNames()
        {
            string[] nameList = new string[ChildrenCount];
            var children = _childTreeMap.Values.ToArray();
            Array.Sort(children);
            for (var i = 0; i < children.Length; i++)
            {
                nameList[i] = children[i].Name;
            }
            return nameList;
        }

        /// <summary>
        /// 关联的数据
        /// </summary>
        public T data = default(T);

        public PathTree()
        {
            Name = string.Empty;
        }

        public PathTree(string name, PathTree<T> parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        /// <summary>
        /// 按照路径创建节点路径（如已有对应的节点，不会覆盖其数据），并返回末端的节点
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public PathTree<T> Create(string[] paths)
        {
            if (null == paths || 0 == paths.Length)
            {
                throw new Exception($"paths 参数错误");
            }

            PathTree<T> child = this;
            for (int i = 0; i < paths.Length; i++)
            {
                child = child.Create(paths[i]);
                if (null == child)
                {
                    throw new Exception($"paths 创建索引:{i} 时错误");
                }
            }

            return child;
        }

        /// <summary>
        /// 创建路径节点并返回
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public PathTree<T> Create(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception($"path 参数错误");
            }

            if (false == _childTreeMap.ContainsKey(path))
            {
                _childTreeMap[path] = new PathTree<T>(path, this);
            }

            return _childTreeMap[path];
        }

        /// <summary>
        /// 清除所有的子节点
        /// </summary>
        public void Clear()
        {
            _childTreeMap.Clear();
        }

        /// <summary>
        /// 沿着路径参数，找到最末端的节点并返回
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="isAccurate">是否精准匹配，开启的话必须严格按照路径匹配的值返回节点（不存在则为null）</param>
        /// <returns></returns>
        public PathTree<T> Find(string[] paths, bool isAccurate = false)
        {
            if (null == paths || 0 == paths.Length)
            {
                throw new Exception($"paths 参数错误");
            }

            PathTree<T> lastNode = this;
            for (int i = 0; i < paths.Length; i++)
            {
                var tempNode = lastNode.Find(paths[i]);
                if (tempNode != null)
                {
                    lastNode = tempNode;
                }
                else
                {
                    if (isAccurate)
                    {
                        lastNode = null;
                    }
                    break;
                }
            }

            return lastNode;
        }

        /// <summary>
        /// 查找路径指向的节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public PathTree<T> Find(string path)
        {
            if (false == _childTreeMap.ContainsKey(path))
            {
                return null;
            }

            return _childTreeMap[path];
        }

        /// <summary>
        /// 将节点的路径字符串形式返回
        /// </summary>
        /// <returns></returns>
        public string ToPathString()
        {
            List<string> pathForwardList = new List<string>();

            PathTree<T> forwardNode = this;
            while (null != forwardNode && false == forwardNode.IsRoot)
            {
                pathForwardList.Add(forwardNode.Name);
                forwardNode = forwardNode.Parent;
            }

            pathForwardList.Reverse();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pathForwardList.Count; i++)
            {
                sb.Append($"/{pathForwardList[i]}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 找到所有的末端节点并返回
        /// </summary>
        /// <returns></returns>
        public List<PathTree<T>> FindLastNodes()
        {
            var list = SearchNodes((node) =>
            {
                if(node.ChildrenCount == 0)
                {
                    return true;
                }
                return false;
            });

            return list;
        }

        /// <summary>
        /// 搜索PathTree下的节点，在func中返回true表示将该节点加入列表
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public List<PathTree<T>> SearchNodes(Func<PathTree<T>, bool> action)
        {
            //遍历所有的节点，并且根据action的返回值确定是否加入到返回列表
            var list = new List<PathTree<T>>();
            IterateThroughNodes(this, (node) =>
            {
                if (action.Invoke(node))
                {
                    list.Add(node);
                }
            });
            return list;
        }

        /// <summary>
        /// 迭代树下所有的节点，并通过回调传递引用。
        /// 注意：传入的PathTree并不会通过回调传递
        /// </summary>
        /// <param name="pathTree"></param>
        /// <param name="onIteratedNode"></param>
        public static void IterateThroughNodes(PathTree<T> pathTree, Action<PathTree<T>> onIteratedNode)
        {
            foreach (var childNode in pathTree._childTreeMap.Values)
            {
                onIteratedNode?.Invoke(childNode);
                IterateThroughNodes(childNode, onIteratedNode);
            }
        }

        /// <summary>
        /// 从指定节点，开始向上查找最近的一个data不为null的节点
        /// </summary>
        /// <returns></returns>
        public static PathTree<T> FindLastNodeWithNonNullDataForward(PathTree<T> pathTree)
        {
            if(pathTree.data != null)
            {
                return pathTree;
            }

            if (pathTree.IsRoot)
            {
                return null;
            }

            return FindLastNodeWithNonNullDataForward(pathTree.Parent);
        }
    }
}
