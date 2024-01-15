using Sirenix.OdinInspector;
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
    class BuildToolsEditorWin : OdinEditorWindow
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = GetWindow<BuildToolsEditorWin>("构建工具", true);
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        BuildToolsConfigVO _vo;

        protected override void OnEnable()
        {
            base.OnEnable();
            _vo = BuildToolsUtility.LoadConfigVO();

            isAutoBuildBuiltinRes = _vo.isAutoBuildBuiltinRes;
        }

        
        [Button("保存配置", ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            _vo.isAutoBuildBuiltinRes = isAutoBuildBuiltinRes;
            BuildToolsUtility.SaveConfigVO(_vo);
            //this.ShowTip("保存完毕");
        }


        [PropertySpace(10)]
        [InfoBox("使用构建工具进行平台构建时，会确保内嵌资源是最新的版本")]
        [ToggleLeft]
        [LabelText("是否自动构建内嵌资源"), PropertyOrder(0)]
        public bool isAutoBuildBuiltinRes = false;

        [Button("构建当前平台", ButtonSizes.Large), PropertyOrder(10)]
        void BuildCurrentPlatform()
        {

        }
    }
}
