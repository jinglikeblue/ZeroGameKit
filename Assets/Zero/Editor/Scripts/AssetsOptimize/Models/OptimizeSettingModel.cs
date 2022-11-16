using Jing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroEditor
{
    /// <summary>
    /// 优化配置模型
    /// </summary>
    class OptimizeSettingModel
    {
        FolderNode root = new FolderNode();

        /// <summary>
        /// 整理纹理优化配置，方便查找
        /// </summary>
        /// <param name="textureSettings"></param>
        public void TidySettings(List<TextureOptimizeSettingVO> settings)
        {
            foreach(var setting in settings)
            {
                root.AddSetting(setting);
            }
        }

        /// <summary>
        /// 找到和路径匹配的配置
        /// </summary>
        /// <param name="path"></param>
        public TextureOptimizeSettingVO FindSetting(string path)
        {
            path = FileUtility.StandardizeBackslashSeparator(path);
            path = FileUtility.RemoveStartPathSeparator(path);
            var paths = path.Split('/');
            for (var i = 0; i < paths.Length; i++)
            {

            }
            return null;
        }


        class FolderNode
        {
            Dictionary<string, FolderNode> childMap= new Dictionary<string, FolderNode>();

            public void AddSetting(TextureOptimizeSettingVO setting)
            {
                var path = FileUtility.StandardizeBackslashSeparator(setting.folder);
                path = FileUtility.RemoveStartPathSeparator(path);
                var paths = path.Split('/');
                FolderNode node = this;
                for (var i = 0; i < paths.Length; i++)
                {
                    node = node.AddChild(paths[i]);
                }
                node.vo = setting;
            }

            public FolderNode AddChild(string folder)
            {
                if (false == childMap.ContainsKey(folder))
                {
                    childMap[folder] = new FolderNode();
                }

                return childMap[folder];
            }

            public FolderNode GetChild(string folder)
            {
                FolderNode value;
                childMap.TryGetValue(folder, out value);
                return value;
            }

            public TextureOptimizeSettingVO vo;
        }
    }
}
