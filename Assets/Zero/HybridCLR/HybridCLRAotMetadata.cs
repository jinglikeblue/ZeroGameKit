using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// HybridCLR补充元数据
    /// </summary>
    public class HybridCLRAotMetadata
    {
        /// <summary>
        /// AOT DLL在Resources目录中的子目录名称
        /// </summary>
        public const string AOT_DLL_RESOURCES_DIR = "hybrid_clr";

        public static void InitAotMetadata()
        {
            LoadMetadataForAOTAssembly();
        }

        /// <summary>
        /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        /// </summary>
        static unsafe void LoadMetadataForAOTAssembly()
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在Huatuo_BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HuatuoData/AssembliesPostIl2CppStrip/{Target} 目录。

            /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误            

            var aotDllList = Resources.LoadAll<TextAsset>(AOT_DLL_RESOURCES_DIR);


            foreach (TextAsset ta in aotDllList)
            {

#if HYBRID_CLR_ENABLE
#if !UNITY_EDITOR
                byte[] dllBytes = ta.bytes;
                fixed (byte* ptr = dllBytes)
                {
                    // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                    int err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                    if(0 == err)
                    {
                        Debug.Log($"补充元数据:{ta.name}  成功");
                    }
                    else
                    {
                        Debug.Log($"补充元数据:{ta.name}  错误码:{err}");
                    }                    
                }
#endif
#else
                Debug.Log($"发现元数据补充文件:{ta.name}");
#endif
            }
        }
    }
}