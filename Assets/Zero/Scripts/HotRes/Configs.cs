using System;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 配置管理工具(对应@ab中的文件)
    /// </summary>
    public class Configs
    {                
        /// <summary>
        /// 加载JSON配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static T LoadJsonConfig<T>(string assetPath)
        {       
            string json = LoadTextConfig(assetPath);
            var vo = Json.ToObject<T>(json);
            return vo;
        }

        /// <summary>
        /// 加载文本配置
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string LoadTextConfig(string assetPath)
        {
            var ta = Res.Load<TextAsset>(assetPath);    
            
            if(null == ta)
            {
                //配置不存在
                throw new Exception(string.Format("[{0}] 文件不存在", assetPath));
            }
            
            return ta.text;
        }

        /// <summary>
        /// 加载二进制数据
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static byte[] LoadBytesConfig(string assetPath)
        {
            var ta = Assets.Load<TextAsset>(assetPath);

            if (null == ta)
            {
                //配置不存在
                throw new Exception(string.Format("[{0}] 文件不存在", assetPath));
            }

            return ta.bytes;
        }

        /// <summary>
        /// 加载自动化工具的配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadZeroConfig<T>()
        {
            var type = typeof(T);

            var atts = type.GetCustomAttributes(typeof(ZeroConfigAttribute), false);
            var att = atts[0] as ZeroConfigAttribute;

            if (null == att)
            {
                //加载的配置不存在
                throw new Exception(string.Format("[{0}] 并不是一个配置数据对象", type.FullName));
            }

            return LoadJsonConfig<T>(att.assetPath);
        }
    }
}
