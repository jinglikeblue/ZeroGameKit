using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroEditor
{
    class AssetsOptimizeEditorWindow : AZeroMenuEditorWindow<AssetsOptimizeEditorWindow>
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = GetWindow<AssetsOptimizeEditorWindow>("AssetImporter 工具", true);
            var rect = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 800);
            win.position = rect;
        }

        protected override void OnEnable()
        {
            menuTree.Config.DrawSearchToolbar = false;
            menuTree.Add("纹理资源优化", new TextureAssetsOptimizeModule(this));
            menuTree.Add("音频资源优化", new AudioAssetsOptimizeModule(this));
            menuTree.Add("资源常量生成", new RClassAutoGenerateModule(this));    
        }
    }
}
