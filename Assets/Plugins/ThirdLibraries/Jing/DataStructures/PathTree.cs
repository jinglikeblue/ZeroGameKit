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
        /// 输出节点信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            List<string> pathForwardList = new List<string>();

            PathTree<T> forwardNode = this;
            while(null != forwardNode && false == forwardNode.IsRoot)
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
        /// 转换完整的树描述信息
        /// </summary>
        /// <returns></returns>
        public string ToFullString()
        {
            var list = FindLastNodes();
            List<string> pathList = new List<string>();            
            foreach(var tree in list)
            {
                var treePath = tree.ToString();
                if (string.IsNullOrEmpty(treePath))
                {
                    continue;
                }

                pathList.Add(treePath);
            }
            pathList.Sort();

            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < pathList.Count; i++)
            {
                sb.AppendLine(pathList[i]);
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// 找到所有的末端节点并返回
        /// </summary>
        /// <returns></returns>
        public List<PathTree<T>> FindLastNodes()
        {
            var list = new List<PathTree<T>>();
            FindLastNodes(list);
            return list;
        }

        void FindLastNodes(List<PathTree<T>> cacheList)
        {
            if(ChildrenCount == 0)
            {
                cacheList.Add(this);
                //末端节点，直接return就行
                return;
            }

            if (ChildrenCount >= 2)
            {
                //分支节点，也是关键节点
                cacheList.Add(this);
            }

            foreach (var tree in _childTreeMap)
            {
                tree.Value.FindLastNodes(cacheList);
            }
        }
    }
}
