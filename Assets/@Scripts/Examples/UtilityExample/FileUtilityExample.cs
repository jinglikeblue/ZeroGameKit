using Jing;
using System.IO;
using System.Text;
using Zero;
using ZeroGameKit;

namespace Example
{
    class FileUtilityExample
    {
        static public void Start()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(LogColor.Zero1("---标准化路径中的路径分隔符（统一使用“\\”符号）"));            
            var tempPath = ZeroConst.PERSISTENT_DATA_PATH;
            var sssPath = FileUtility.StandardizeSlashSeparator(tempPath);
            sb.AppendLine($"路径：{LogColor.Orange(sssPath)}   存在：{Directory.Exists(sssPath)}");
            sb.AppendLine(LogColor.Zero1("---标准化路径中的路径分隔符（统一使用“/”符号）"));
            var sbsPath = FileUtility.StandardizeBackslashSeparator(sssPath);
            sb.AppendLine($"路径：{LogColor.Orange(sbsPath)}   存在：{Directory.Exists(sbsPath)}");
            sb.AppendLine(LogColor.Zero1("---将给的目录路径合并起来"));
            var dir = FileUtility.CombineDirs(true, new string[] { sbsPath, "example", "file_utility" });
            sb.AppendLine(LogColor.Orange(dir));
            sb.AppendLine(LogColor.Zero1("---将给的路径合并起来"));
            var testFilePath = FileUtility.CombinePaths(new string[] { dir, "files", "test.txt" });
            sb.AppendLine(LogColor.Orange(testFilePath));

            sb.AppendLine(LogColor.Zero1("---创建文件"));
            var testFileInfo = new FileInfo(testFilePath);
            if (!testFileInfo.Directory.Exists)
            {
                //创建目录
                testFileInfo.Directory.Create();
            }
            File.WriteAllText(testFilePath, "Hello World");
            sb.AppendLine($"路径：{LogColor.Orange(testFilePath)}   存在：{File.Exists(testFilePath)}");

            sb.AppendLine(LogColor.Zero1("---获取文件相对于[ZeroConst.PERSISTENT_DATA_PATH]的路径"));
            sb.AppendLine($"路径：{LogColor.Orange(FileUtility.GetRelativePath(ZeroConst.PERSISTENT_DATA_PATH, testFilePath))}");

            sb.AppendLine(LogColor.Zero1("---删除目录下使用指定扩展名的文件"));
            FileUtility.DeleteFilesByExt(ZeroConst.PERSISTENT_DATA_PATH, "txt", SearchOption.AllDirectories);
            sb.AppendLine($"路径：{LogColor.Orange(testFilePath)}   存在：{File.Exists(testFilePath)}");

            var msg = MsgWin.Show("FileUtility", sb.ToString());
            msg.SetContentAlignment(UnityEngine.TextAnchor.UpperLeft);
        }
    }
}
