using System.Collections.Generic;
using System.Linq;

namespace ZeroEditor.IOS
{
    public abstract class BaseXCodeConfigEditorModule : AEditorModule
    {
        public const string ConfigFile = "ios_project_config.json";

        private static IOSProjectInitConfigVO _cfg;

        protected BaseXCodeConfigEditorModule(UnityEditor.EditorWindow editorWin) : base(editorWin)
        {
        }

        /// <summary>
        /// 配置数据
        /// </summary>
        protected IOSProjectInitConfigVO Cfg
        {
            get
            {
                if (null == _cfg)
                {
                    LoadConfig();
                }

                return _cfg;
            }
        }

        private static void LoadConfig()
        {
            if (null != _cfg)
            {
                return;
            }

            _cfg = EditorConfigUtil.LoadConfig<IOSProjectInitConfigVO>(ConfigFile);
            if (null == _cfg)
            {
                _cfg = new IOSProjectInitConfigVO();
            }
        }

        protected void SaveConfigFile()
        {
            EditorConfigUtil.SaveConfig(_cfg, ConfigFile);
            editorWin.ShowTip("保存成功!");
        }
        
        /// <summary>
        /// 检查Dictionary，把value为null的值都替换为string.Empty
        /// </summary>
        /// <param name="dict"></param>
        protected void ReplaceDictValueNullValue(Dictionary<string, string> dict)
        {
            if (dict == null) return;

            var keys = dict.Keys.ToArray();
            foreach (var key in keys)
            {
                if (null == dict[key])
                {
                    dict[key] = string.Empty;
                }
            }
        }
    }
}