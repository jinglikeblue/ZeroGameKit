using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// HybridCLR����Ԫ����
    /// </summary>
    public class HybridCLRAotMetadata
    {
        /// <summary>
        /// AOT DLL��ResourcesĿ¼�е���Ŀ¼����
        /// </summary>
        public const string AOT_DLL_RESOURCES_DIR = "hybrid_clr";

        public static void InitAotMetadata()
        {
            LoadMetadataForAOTAssembly();
        }

        /// <summary>
        /// Ϊaot assembly����ԭʼmetadata�� ��������aot�����ȸ��¶��С�
        /// һ�����غ����AOT���ͺ�����Ӧnativeʵ�ֲ����ڣ����Զ��滻Ϊ����ģʽִ��
        /// </summary>
        static unsafe void LoadMetadataForAOTAssembly()
        {
            // ���Լ�������aot assembly�Ķ�Ӧ��dll����Ҫ��dll������unity build���������ɵĲü����dllһ�£�������ֱ��ʹ��ԭʼdll��
            // ������Huatuo_BuildProcessor_xxx������˴�����룬��Щ�ü����dll�ڴ��ʱ�Զ������Ƶ� {��ĿĿ¼}/HuatuoData/AssembliesPostIl2CppStrip/{Target} Ŀ¼��

            /// ע�⣬����Ԫ�����Ǹ�AOT dll����Ԫ���ݣ������Ǹ��ȸ���dll����Ԫ���ݡ�
            /// �ȸ���dll��ȱԪ���ݣ�����Ҫ���䣬�������LoadMetadataForAOTAssembly�᷵�ش���            

            var aotDllList = Resources.LoadAll<TextAsset>(AOT_DLL_RESOURCES_DIR);


            foreach (TextAsset ta in aotDllList)
            {

#if HYBRID_CLR_ENABLE
#if !UNITY_EDITOR
                byte[] dllBytes = ta.bytes;
                fixed (byte* ptr = dllBytes)
                {
                    // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
                    int err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                    if(0 == err)
                    {
                        Debug.Log($"����Ԫ����:{ta.name}  �ɹ�");
                    }
                    else
                    {
                        Debug.Log($"����Ԫ����:{ta.name}  ������:{err}");
                    }                    
                }
#endif
#else
                Debug.Log($"����Ԫ���ݲ����ļ�:{ta.name}");
#endif
            }
        }
    }
}