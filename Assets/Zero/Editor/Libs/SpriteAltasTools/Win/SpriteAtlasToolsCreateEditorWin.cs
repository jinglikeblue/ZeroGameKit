using Jing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// SpriteAtlas创建工具
    /// </summary>
    class SpriteAtlasToolsCreateEditorWin : OdinEditorWindow
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirPath">打开窗口时，默认关联的目录</param>
        /// <returns></returns>
        public static SpriteAtlasToolsCreateEditorWin Open(string dirPath = null)
        {
            return GetWindow<SpriteAtlasToolsCreateEditorWin>("SpriteAtlas Creater");
        }
    }
}