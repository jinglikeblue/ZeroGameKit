using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace ZeroEditor
{
    /// <summary>
    /// 纹理资源优化模块
    /// </summary>
    class TextureAssetsOptimizeModule : AEditorModule
    {
        public TextureAssetsOptimizeModule(EditorWindow editorWin) : base(editorWin)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();

            isEnable = AssetsOptimizeUtility.Config.isTextureOptimizeEnable;
            settings = AssetsOptimizeUtility.Config.textureSettings;
        }

        [PropertyOrder(0)]
        [Button("保存配置", ButtonSizes.Large)]
        void SaveConfig()
        {
            AssetsOptimizeUtility.Config.isTextureOptimizeEnable = isEnable;
            AssetsOptimizeUtility.Config.textureSettings = settings;
            AssetsOptimizeUtility.SaveConfig();
        }

        [PropertyOrder(100)]
        [InfoBox("开启后：资源第一次导入Unity时，会进行自动配置")]
        [LabelText("自动优化功能是否启用")]
        public bool isEnable = false;

        [PropertyOrder(200)]
        [LabelText("优化配置")]
        [ShowInInspector]
        [ListDrawerSettings(Expanded = true, ListElementLabelName = "folder", AlwaysAddDefaultValue = true, DraggableItems = false, CustomAddFunction = "AddTextureOptimizeSetting", CustomRemoveElementFunction = "RemoveTextureOptimizeSetting")]
        public List<TextureOptimizeSettingVO> settings;

        void AddTextureOptimizeSetting()
        {
            string folder = EditorUtility.OpenFolderPanel("选择配置的目录", "Assets", "");
            if (string.IsNullOrEmpty(folder))
            {
                return;
            }

            folder = FileUtil.GetProjectRelativePath(folder);

            foreach(var setting in settings)
            {
                if(setting.folder == folder)
                {
                    EditorUtility.DisplayDialog("添加失败", "已有该目录的配置信息!", "确认");
                    return;
                }
            }

            var vo = new TextureOptimizeSettingVO();
            vo.folder = folder;

            settings.Add(vo);
        }

        void RemoveTextureOptimizeSetting(TextureOptimizeSettingVO vo)
        {
            if (EditorUtility.DisplayDialog("删除配置", "是否确定删除该配置?", "确认", "取消"))
            {
                settings.Remove(vo);
            }
        }

        [PropertyOrder(300)]
        [InfoBox("按照配置对资源进行一次配置")]
        [Button("执行优化", ButtonSizes.Large)]
        void OptimizeAll()
        {
            if (!EditorUtility.DisplayDialog("提示", "是否立刻执行优化设置?", "确认", "取消"))
            {
                return;
            }

            SaveConfig();
            AssetsOptimizeUtility.OptimizeTextures();
        }

        //[Button("测试")]
        //void Test()
        //{
        //    AssetsOptimizeUtility.OptimizeTexturesFolder("Assets/Examples/Art/Icons");
        //}

    }
}
