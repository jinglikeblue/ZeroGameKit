
using System.Collections.Generic;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// SpriteAtlasTools配置数据对象
    /// </summary>
    class SpriteAtlasToolsConfigVO
    {
        /// <summary>
        /// 创建的spriteatlas文件保存的目录
        /// </summary>
        public string spriteAtlasSaveDirPath = ZeroConst.PROJECT_AB_DIR + "/spriteatlas";

        /// <summary>
        /// 打包到纹理集的资源最大宽度限制
        /// </summary>
        public int packingTextureWidthLimit = 1024;

        /// <summary>
        /// 打包到纹理集的资源最大高度限制
        /// </summary>
        public int packingTextureHeightLimit = 1024;

        /// <summary>
        /// 配置的spriteatlas文件数据
        /// </summary>
        public List<SpriteAtlasItemVO> itemList = new List<SpriteAtlasItemVO>();

        public class SpriteAtlasItemVO
        {
            /// <summary>
            /// 打包纹理集的目录
            /// </summary>
            public string texturesDirPath;

            /// <summary>
            /// 是否子目录单独创建spriteatlas
            /// </summary>
            public bool isSubDirSplit = false;
        }
    }
}