using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// 声音优化配置文件
    /// </summary>
    [ReadOnly]
    [HideReferenceObjectPicker]
    public class AudioOptimizeSettingVO : IOptimizeSettingVO
    {
        /// <summary>
        /// 对应的文件夹(如果另外配置了该文件夹的子目录，则子目录的配置会覆盖该配置)
        /// </summary>
        [LabelText("对应的文件夹"), ReadOnly, FolderPath]
        [GUIColor(0.3f, 0.8f, 0f, 1f)]
        public string folder;
        public string GetFolder()
        {
            return folder;
        }

        /// <summary>
        /// 打包时，将音频合并为单通道
        /// </summary>
        [LabelText("Force To Mono")]
        public bool forceToMono = false;

        /// <summary>
        /// 在后台单独线程中加载，不会阻塞主线程
        /// </summary>
        [LabelText("Load In Background")]
        public bool loadInBackground = false;

        /// <summary>
        /// 预加载数据到内存
        /// 只有在Load Type 选中为 Decompress On Load/ Compressed In Memory时有用。开启后，场景加载时会被预加载到内存。
        /// </summary>
        [LabelText("Preload Audio Data")]
        public bool preloadAudioData = true;

        /// <summary>
        /// Decompress On Load: 音频加载到内存后立即解压。（适用使用频率高，且时间较短的音效）
        /// Compressed In Memory: 音频加载到内存后不解压，播放时解压。
        /// Streaming: 动态解码声音。内容中会缓冲磁盘上的音频文件的压缩数据。即使没有加载音频文件，也会占有一个200KB的空间。
        /// </summary>
        [LabelText("Load Type")]
        public AudioClipLoadType loadType = AudioClipLoadType.DecompressOnLoad;

        /// <summary>
        /// 音频的打包格式
        /// </summary>
        [LabelText("Compression Format")]
        public AudioCompressionFormat compressionFormat = AudioCompressionFormat.Vorbis;

        /// <summary>
        /// 压缩质量
        /// </summary>
        [LabelText("Quality")]
        [PropertyRange(0,100)]
        public int quality = 100;



    }
}
