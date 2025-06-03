using Sirenix.Utilities.Editor;
using System.Reflection;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// 配置面板
    /// </summary>
    class HotConfigEditorWin : AZeroMenuEditorWindow<HotConfigEditorWin>
    {
        protected override void OnEnable()
        {            
            var types = Assembly.GetAssembly(typeof(Zero.ZeroConfigAttribute)).GetTypes();
            foreach (var type in types)
            {
                var att = type.GetCustomAttribute<ZeroConfigAttribute>(false);
                if (null != att)
                {                    
                    menuTree.Add(att.label, new HotConfigModule(type, att.assetPath, this), EditorIcons.File);
                }
            }
        }
    }


}
