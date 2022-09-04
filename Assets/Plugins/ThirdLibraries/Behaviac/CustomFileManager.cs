using behaviac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace behaviac
{
    public class CustomFileManager : FileManager
    {
        public override void FileClose(string filePath, string ext, byte[] pBuffer)
        {
            Resources.UnloadUnusedAssets();
            //base.FileClose(filePath, ext, pBuffer);
        }

        public override byte[] FileOpen(string filePath, string ext)
        {
            var assetName = $"{filePath}{ext}";
            assetName = Path.GetFileName(assetName);
            var bytes = Resources.Load<TextAsset>(assetName).bytes;
            return bytes;
            //return base.FileOpen(filePath, ext);
        }
    }
}
