﻿using System;
using System.IO;

namespace Jing
{
    /// <summary>
    /// 文件处理工具
    /// </summary>
    public class FileUtility
    {
                public enum EPathType
        {
            FILE,
            DIRECTORY,
            OTHER
        }

        /// <summary>
        /// 标准化路径中的路径分隔符（统一使用“/”符号）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string StandardizeBackslashSeparator(string path)
        {
            path = path.Replace("\\", "/");
            return path;
        }

        /// <summary>
        /// 标准化路径中的路径分隔符（统一使用“\”符号）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string StandardizeSlashSeparator(string path)
        {
            path = path.Replace("/", "\\");
            return path;
        }

        /// <summary>
        /// 删除目录下使用指定扩展名的文件
        /// </summary>
        /// <param name="dirPath">目录地址</param>
        /// <param name="ext">扩展名 格式可以为[exe]或[.exe]</param>
        /// <param name="searchOption">指定是搜索当前目录，还是搜索当前目录及其所有子目录</param>
        public static void DeleteFilesByExt(string dirPath, string ext, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (false == ext.StartsWith("."))
            {
                ext = "." + ext;
            }            

            string[] files = Directory.GetFiles(dirPath, "*" + ext, searchOption);
            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    if (Path.GetExtension(file) == ext)
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        /// <summary>
        /// 将给的路径合并起来
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string CombinePaths(params string[] args)
        {
            if (args.Length == 0)
            {
                return "";
            }

            string path = args[0];
            for (int i = 1; i < args.Length; i++)
            {
                var node = RemoveStartPathSeparator(args[i]);
                path = Path.Combine(path, node);
            }

            //为了好看
            path = StandardizeBackslashSeparator(path);

            return path;
        }

        /// <summary>
        /// 将给的目录路径合并起来
        /// </summary>
        /// <param name="endWithBackslash">路径最后是否以反斜杠结束</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string CombineDirs(bool isEndWithBackslash, params string[] args)
        {
            string path = CombinePaths(args);

            if (isEndWithBackslash)
            {
                if (false == path.EndsWith("/"))
                {
                    path += "/";
                }
            }
            else
            {
                if (path.EndsWith("/"))
                {
                    path = path.Substring(0, path.Length - 1);
                }
            }

            return path;
        }

        /// <summary>
        /// 如果路径开头有文件分隔符，则移除
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveStartPathSeparator(string path)
        {
            if (path.StartsWith("/"))
            {
                return path.Substring(1);
            }
            else if (path.StartsWith("\\"))
            {
                return path.Substring(2);
            }

            return path;
        }

        /// <summary>
        /// 如果路径结尾有文件分隔符，则移除
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveEndPathSeparator(string path)
        {
            if (path.EndsWith("/"))
            {
                return path.Substring(1);
            }
            else if (path.EndsWith("\\"))
            {
                return path.Substring(2);
            }

            return path;
        }

        /// <summary>
        /// 检查是否允许拷贝的委托
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        /// <returns></returns>
        public delegate bool CheckCopyEnableDelegate(string sourceFile, string targetFile);

        public static void CopyDir(string source, string target, CheckCopyEnableDelegate checkCopyEnable = null, Action<string> onFileCopied = null)
        {
            source = StandardizeBackslashSeparator(source);
            target = StandardizeBackslashSeparator(target);

            if (false == Directory.Exists(source))
            {
                throw new Exception(string.Format("文件夹不存在:[{0}]", source));
            }

            var subFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            for (int i = 0; i < subFiles.Length; i++)
            {
                var subFile = StandardizeBackslashSeparator(subFiles[i]);
                var subFileRelativePath = subFile.Replace(source, "");
                var targetFile = CombinePaths(target, subFileRelativePath);
                bool copyEnable = true;
                if (checkCopyEnable != null)
                {
                    copyEnable = checkCopyEnable(subFile, targetFile);
                }

                if (copyEnable)
                {
                    CopyFile(subFile, targetFile, true);
                    onFileCopied?.Invoke(targetFile);
                }
            }
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="overwrite"></param>
        public static void CopyFile(string source, string target, bool overwrite = true)
        {
            if (false == File.Exists(source))
            {
                throw new Exception(string.Format("文件不存在:[{0}]", source));
            }

            var targetDir = Directory.GetParent(target);
            if (false == targetDir.Exists)
            {
                targetDir.Create();
            }

            File.Copy(source, target, overwrite);
        }

        /// <summary>
        /// 获取targetDir相对于startDir的目录地址
        /// </summary>
        /// <param name="startDir"></param>
        /// <param name="targetDir"></param>
        /// <returns></returns>
        public static string GetRelativePath(string startDir, string targetDir)
        {
            //首先统一路径为反斜杠
            startDir = CombineDirs(false, startDir);
            targetDir = CombineDirs(false, targetDir);

            if (startDir == targetDir)
            {
                return "./";
            }

            var minLength = startDir.Length < targetDir.Length ? startDir.Length : targetDir.Length;

            int i;
            //先找出共同的部分
            for (i = 0; i < minLength; i++)
            {
                if (startDir[i] != targetDir[i])
                {
                    break;
                }
            }

            if (i == 0)
            {
                //根本没有相同的字符，不存在相对路径
                return null;
            }

            //截取出公共的部分
            var commonPart = startDir.Substring(0, i);
            //获取移除掉公共部分的内容
            startDir = startDir.Replace(commonPart, "");
            targetDir = targetDir.Replace(commonPart, "");
            //移除开头的反斜杠
            if (startDir.StartsWith("/"))
            {
                startDir = startDir.Substring(1);
            }
            if (targetDir.StartsWith("/"))
            {
                targetDir = targetDir.Substring(1);
            }

            string result = null;
            if (startDir.Length == 0)
            {
                result = targetDir;
            }
            else if (targetDir.Length == 0)
            {
                var count = startDir.Split('/').Length;
                result = "";
                while (--count > -1)
                {
                    result += "../";
                }
            }
            else
            {
                var count = startDir.Split('/').Length;
                result = "";
                while (--count > -1)
                {
                    result += "../";
                }
                result += targetDir;
            }

            return result;
        }
    

        /// <summary>
        /// 检查路径是否指向文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckIsFile(string path)
        {
            return File.Exists(path);
        }
        
        /// <summary>
        /// 检查路径是否指向文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckIsDirectory(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// 检查路径对应的文件类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static EPathType CheckPathType(string path)
        {
            if (CheckIsFile(path))
            {
                return EPathType.FILE;
            }

            if (CheckIsDirectory(path))
            {
                return EPathType.DIRECTORY;
            }

            return EPathType.OTHER;
        }
    }
}