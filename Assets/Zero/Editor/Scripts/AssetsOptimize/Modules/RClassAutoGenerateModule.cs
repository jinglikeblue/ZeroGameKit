using Jing;
using Sirenix.OdinInspector;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// 资源常量文件自动生成模块
    /// </summary>
    public class RClassAutoGenerateModule : AEditorModule
    {
        public RClassAutoGenerateModule(UnityEditor.EditorWindow editorWin) : base(editorWin)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();

            settings = ClassUtility.DeepClone(AssetsOptimizeUtility.Config.rClassAutoGenerateSetting);
        }

        [PropertyOrder(0)]
        [Button("保存配置", ButtonSizes.Large)]
        void SaveConfig()
        {
            ClassUtility.CopyTo(settings, AssetsOptimizeUtility.Config.rClassAutoGenerateSetting);
            AssetsOptimizeUtility.SaveConfig(false);
        }

        [PropertyOrder(200)]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public RClassAutoGenerateSettingVO settings;
    }
}