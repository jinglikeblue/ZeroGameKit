using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroEditor
{
    /// <summary>
    /// iOS项目初始化配置
    /// </summary>
    public class IOSProjectInitConfigVO
    {
        /// <summary>
        /// 要拷贝的文件
        /// </summary>
        public CopyInfoVO[] copyInfoList = new CopyInfoVO[0];


        #region PBXProject

        /// <summary>
        /// 对应pbx.GetUnityMainTargetGuid()
        /// </summary>
        public PBXProjectSettingVO main = new PBXProjectSettingVO();

        /// <summary>
        /// 对应pbx.GetUnityFrameworkTargetGuid()
        /// 从Unity2019LTS开始，XCode项目分为主工程和库项目两部分
        /// </summary>
        public PBXProjectSettingVO framework = new PBXProjectSettingVO();

        #endregion

        #region info.plist

        /// <summary>
        /// Info.plist参数配置
        /// </summary>
        public Dictionary<string, string> pListDataList = new Dictionary<string, string>();

        /// <summary>
        /// urlscheme配置
        /// </summary>
        public string[] urlSchemeList = new string[0];

        /// <summary>
        /// 信任urlscheme配置
        /// </summary>
        public string[] appQueriesSchemeList = new string[0];

        #endregion


        public class PBXProjectSettingVO
        {
            /// <summary>
            /// framework库配置
            /// </summary>
            public string[] frameworkToProjectList = new string[0];

            /// <summary>
            /// lib库配置
            /// </summary>
            public Dictionary<string, string> file2BuildList = new Dictionary<string, string>();

            /// <summary>
            /// build参数配置
            /// </summary>
            public Dictionary<string, string> buildPropertyList = new Dictionary<string, string>();
        }

        public class CopyInfoVO
        {
            /// <summary>
            /// 相对于Unity工程目录的路径
            /// </summary>
            public string fromPath;

            /// <summary>
            /// 相对于XCode工程目录的路径
            /// </summary>
            public string toPath;

            /// <summary>
            /// 是否添加到Main的编译中
            /// </summary>
            public bool isAddToMain = false;

            /// <summary>
            /// 是否添加到Framework的编译中
            /// </summary>
            public bool isAddToFramework = false;

            /// <summary>
            /// 是否将文件或目录添加到【Build Phases】的【Copy Bundle Resources】中
            /// </summary>
            public bool isAddPathToResourcesBuildPhase = false;
        }

    }
}
