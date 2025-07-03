using Jing;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor.IOS
{
    class IOSCopyFilesToXCodeModule : BaseXCodeConfigEditorModule
    {
        public IOSCopyFilesToXCodeModule(EditorWindow editorWin) : base(editorWin)
        {
            foreach (var item in Cfg.copyInfoList)
            {
                copyFileList.Add(new CopyFile(item));
            }
        }

        [Title("Info.plist 配置", titleAlignment: TitleAlignments.Centered)]
        [Button("保存配置", ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            CacheConfig();
            SaveConfigFile();
        }

        void CacheConfig()
        {
            base.OnDisable();
            List<IOSProjectInitConfigVO.CopyInfoVO> list = new List<IOSProjectInitConfigVO.CopyInfoVO>();
            foreach (var item in copyFileList)
            {
                list.Add(item.VO);
            }

            Cfg.copyInfoList = list.ToArray();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            CacheConfig();
        }

        [Space(20)]
        [LabelText("拷贝文件")]
        [ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 5, Expanded = true, ShowIndexLabels = true)]
        public List<CopyFile> copyFileList = new List<CopyFile>();

        [Serializable]
        public class CopyFile
        {
            IOSProjectInitConfigVO.CopyInfoVO _vo;

            public IOSProjectInitConfigVO.CopyInfoVO VO
            {
                get
                {
                    if (null == _vo)
                    {
                        _vo = new IOSProjectInitConfigVO.CopyInfoVO();
                    }

                    _vo.fromPath = fromPath;
                    _vo.toPath = toPath;
                    _vo.isAddToMain = isAddToMain;
                    _vo.isAddToFramework = isAddToFramework;
                    _vo.isAddPathToResourcesBuildPhase = isAddPathToResourcesBuildPhase;

                    return _vo;
                }
            }

            public CopyFile(IOSProjectInitConfigVO.CopyInfoVO vo)
            {
                this._vo = vo;
                fromPath = vo.fromPath;
                toPath = vo.toPath;
                isAddToMain = vo.isAddToMain;
                isAddToFramework = vo.isAddToFramework;
                isAddPathToResourcesBuildPhase = vo.isAddPathToResourcesBuildPhase;
            }

            [InfoBox("相对于Unity工程目录的路径(可以指向文件或文件夹)", InfoMessageType.None)]
            /// <summary>
            /// 相对于Unity工程目录的路径
            /// </summary>
            public string fromPath;

            [InfoBox("相对于XCode工程目录的路径(可以指向文件或文件夹)", InfoMessageType.None)]
            /// <summary>
            /// 相对于XCode工程目录的路径
            /// </summary>
            public string toPath;

            [InfoBox("是否添加到Main的编译中", InfoMessageType.None)]
            public bool isAddToMain;

            [InfoBox("是否添加到Framework的编译中", InfoMessageType.None)]
            public bool isAddToFramework;

            [ShowIf("@isAddToMain||isAddToFramework")]
            [InfoBox("是否将文件或目录添加到【Build Phases】的【Copy Bundle Resources】中", InfoMessageType.None)]
            [InfoBox("【谨慎使用该选项】Unity没有自动完成添加的情况才需要勾选该选项。否则可能造成文件的重复添加。", InfoMessageType.Warning, VisibleIf = "isAddPathToResourcesBuildPhase")]
            public bool isAddPathToResourcesBuildPhase;
        }
    }
}