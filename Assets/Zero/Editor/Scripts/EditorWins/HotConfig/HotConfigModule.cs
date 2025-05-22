using Sirenix.OdinInspector;
using System;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Text;
using Jing;
using System.IO;
using Zero;
using UnityEngine;

namespace ZeroEditor
{
    class HotConfigModule : AEditorModule
    {
        string _path;

        public HotConfigModule(Type type, string path, EditorWindow editorWin) : base(editorWin)
        {
            var attributes = type.GetCustomAttributes(typeof(ZeroConfigAttribute),true);
            var assetPath = (attributes[0] as ZeroConfigAttribute).assetPath;
            _path = Res.TransformToProjectPath(assetPath, EResType.Asset);

            if (File.Exists(_path))
            {
                try
                {
                    string json = File.ReadAllText(_path, Encoding.UTF8);                    
                    vo = Json.ToObject(json, type);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            else
            {
                vo = Activator.CreateInstance(type);
            }
        }

        [ShowInInspector]
        [HideReferenceObjectPicker]
        public object vo;

        [Button("保存", ButtonSizes.Large), PropertyOrder(-1)]
        public void Save()
        {
            string json = Json.ToJsonIndented(vo);
            json = Regex.Unescape(json);
            var dir = Directory.GetParent(_path);
            if (!dir.Exists)
            {
                dir.Create();
            }
            File.WriteAllText(_path, json, Encoding.UTF8);
            AssetDatabase.Refresh();
        }
    }
}
