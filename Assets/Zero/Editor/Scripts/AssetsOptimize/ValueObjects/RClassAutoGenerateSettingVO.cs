using Sirenix.OdinInspector;

namespace ZeroEditor
{
    /// <summary>
    /// 资源常量类自动化生成配置数据
    /// </summary>
    [HideReferenceObjectPicker]
    [HideLabel]
    public class RClassAutoGenerateSettingVO
    {
        [InfoBox("在热更资源目录中的文件有增加、删除的情况下，自动构建生成R.cs", InfoMessageType.Warning)]
        [LabelText("是否开启自动生成资源常量类")]
        public bool isAutoGenerateEnable = false;
    }
}