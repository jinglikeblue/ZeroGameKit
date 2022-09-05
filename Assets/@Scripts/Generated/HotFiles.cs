//该类由 [Zero → 自动生成代码 → Assets资源名生成] 工具自动创建
using Jing;
using System;
using System.Collections.Generic;
using System.IO;

namespace ZeroHot
{
    /// <summary>
    /// [自动生成的]Files目录下的文件数据
    /// </summary>
    public sealed class HotFiles
    {
        
        public const string READ_ME_TXT = "read_me.txt";

        public const string _隐私政策_TXT = "隐私政策.txt";

        public const string PICS_功能_PNG = "pics/功能.png";

        public const string PICS_SCREENSHOTS_DEMO_0_PNG = "pics/ScreenShots/Demo_0.png";

        public const string PICS_SCREENSHOTS_DEMO_1_PNG = "pics/ScreenShots/Demo_1.png";

        public const string PICS_SCREENSHOTS_DEMO_2_PNG = "pics/ScreenShots/Demo_2.png";

        public const string PICS_SCREENSHOTS_NET_PNG = "pics/ScreenShots/Net.png";

        public const string PICS_SCREENSHOTS_SCROLLLIST_PNG = "pics/ScreenShots/ScrollList.png";

        public const string PICS_SCREENSHOTS_VIDEO_PNG = "pics/ScreenShots/Video.png";

        public const string VIDEOS_SAMPLE_MP4 = "videos/Sample.mp4";


        static string[] allFilesList = new string[]
        {
            
            READ_ME_TXT,
            _隐私政策_TXT,
            PICS_功能_PNG,
            PICS_SCREENSHOTS_DEMO_0_PNG,
            PICS_SCREENSHOTS_DEMO_1_PNG,
            PICS_SCREENSHOTS_DEMO_2_PNG,
            PICS_SCREENSHOTS_NET_PNG,
            PICS_SCREENSHOTS_SCROLLLIST_PNG,
            PICS_SCREENSHOTS_VIDEO_PNG,
            VIDEOS_SAMPLE_MP4,
        };        	

        /// <summary>
        /// 获取所有文件的列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllFileList()
        {
            var list = new string[allFilesList.Length];
            Array.Copy(allFilesList, list, allFilesList.Length);
            return list;
        }

        /// <summary>
        /// 判断路径是否指向的一个文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static bool IsFile(string path)
        {
            return path.Contains(".");
        }

        /// <summary>
        /// 判断路径是否是一个文件名，而不是一个路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static bool IsFileName(string path)
        {
            path = FileUtility.StandardizeBackslashSeparator(path);
            if (path.Contains("/"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取目录下的文件
        /// </summary>
        /// <param name="dirPath">目录路径</param>
        /// <param name="searchOption">返回方式，TopDirectoryOnly：仅指定目录；AllDirectories：包含子目录中的文件 </param>
        /// <returns></returns>
        public static string[] GetFiles(string dirPath, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            List<string> list = new List<string>();

            dirPath = FileUtility.CombineDirs(true, dirPath);
            foreach(var file in allFilesList)
            {
                if (file.StartsWith(dirPath))
                {
                    switch (searchOption)
                    {                        
                        case SearchOption.AllDirectories:
                            list.Add(file);
                            break;
                        case SearchOption.TopDirectoryOnly:
                            var remainPath = file.Remove(0, dirPath.Length);
                            if (IsFileName(remainPath))
                            {
                                list.Add(file);
                            }
                            break;
                    }
                }
            }
            return list.ToArray();
        }        
    }    
}
