using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroEditor;

namespace ZeroEditor
{
    class BuildToolsUtility
    {
        /// <summary>
        /// 配置文件位置
        /// </summary>
        public const string CONFIG_NAME = "build_tools_config.json";

        public static BuildToolsConfigVO LoadConfigVO()
        {
            var vo = EditorConfigUtil.LoadConfig<BuildToolsConfigVO>(CONFIG_NAME);
            return vo;
        }

        public static void SaveConfigVO(BuildToolsConfigVO vo)
        {
            EditorConfigUtil.SaveConfig(vo, CONFIG_NAME);
        }
    }
}
