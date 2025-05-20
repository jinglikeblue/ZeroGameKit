using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jing;
using UnityEngine;
using Zero;


namespace ZeroEditor
{
    /// <summary>
    /// 文件名映射模型。
    /// 用来处理文件路径已经映射Key的构建
    /// </summary>
    public class FilePathMappingModel
    {
        private readonly string[] _files;

        private readonly Dictionary<string, List<string>> _duplicatesDict = new Dictionary<string, List<string>>();

        private readonly BidirectionalMap<string, string> _mappingDict = new BidirectionalMap<string, string>();

        public string[] GetNames()
        {
            return _mappingDict.GetLefts();
        }

        public string[] GetPaths()
        {
            return _mappingDict.GetRights();
        }

        public string GetName(string path)
        {
            return _mappingDict.GetLeft(path);
        }

        public string GetPath(string name)
        {
            return _mappingDict.GetRight(name);
        }

        public FilePathMappingModel(string[] files)
        {
            _files = files;

            CreateDuplicatesDict();
            CreateMappingDict();
        }

        private void CreateDuplicatesDict()
        {
            var fileDict = new Dictionary<string, List<string>>();

            for (int i = 0; i < _files.Length; i++)
            {
                var path = _files[i];
                string fileName = Path.GetFileName(path);
                if (fileDict.ContainsKey(fileName))
                {
                    fileDict[fileName].Add(path);
                }
                else
                {
                    fileDict[fileName] = new List<string> { path };
                }
            }

            var duplicates = fileDict.Where(kv => kv.Value.Count > 1);

            foreach (var kv in duplicates)
            {
                _duplicatesDict.Add(kv.Key, kv.Value);
            }
        }

        private void CreateMappingDict()
        {
            foreach (var path in _files)
            {
                var fileName = Path.GetFileName(path);
                if (_duplicatesDict.ContainsKey(fileName))
                {
                    //因为是重复文件，所以需要生成一个带路径的Key
                    fileName = CreateUniqueKey(path);
                }

                fileName = BaseGenerateTemplateCodeCommand.MakeFieldNameRightful(fileName);

                if (_mappingDict.ContainsLeft(fileName))
                {
                    Debug.LogError($"重复文件名：({fileName})[{path}]");
                }
                else
                {
                    _mappingDict.Set(fileName, path);
                }
            }
        }

        private string CreateUniqueKey(string path)
        {
            var fileName = Path.GetFileName(path);
            var selfFolderNames = SplitFolderNames(path);
            var otherFolderNamesList = new List<string[]>();
            foreach (var tempPath in _duplicatesDict[fileName])
            {
                if (tempPath.Equals(path))
                {
                    continue;
                }

                otherFolderNamesList.Add(SplitFolderNames(tempPath));
            }

            List<string> flags = new List<string>();
            for (int i = 0; i < selfFolderNames.Length; i++)
            {
                int sameCount = 0;
                var flag = selfFolderNames[i];
                foreach (var otherFolderNames in otherFolderNamesList)
                {
                    if (flag.Equals(otherFolderNames[i]))
                    {
                        sameCount++;
                    }
                }

                //所有的重复文件名，该位置都是重复的，所以不用作为Key的一部分，忽略掉
                if (sameCount == otherFolderNamesList.Count)
                {
                    continue;
                }

                flags.Add(flag);
                if (0 == sameCount)
                {
                    break;
                }
            }

            flags.Reverse();
            var key = string.Join("_", flags);
            return $"{key}_{fileName}";
        }

        string[] SplitFolderNames(string path)
        {
            var folder = FileUtility.StandardizeBackslashSeparator(Path.GetDirectoryName(path));
            var folderNames = folder.Split("/").Reverse().ToArray();
            return folderNames;
        }
    }
}