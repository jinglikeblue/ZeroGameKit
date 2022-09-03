using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    public class HuaTuoAotMetadata
    {
        /// <summary>
        /// ��٢��AOT DLL����Resources�µ���Ŀ¼����
        /// </summary>
        static public readonly string HUATUO_RESOURCES_DIR = "huatuo";

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
            
            var aotDllList = Resources.LoadAll<TextAsset>(HUATUO_RESOURCES_DIR);


            foreach (TextAsset ta in aotDllList)
            {                
                byte[] dllBytes = ta.bytes;
                fixed (byte* ptr = dllBytes)
                {
#if !UNITY_EDITOR && UNITY_ENABLE
                    // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
                    int err = Huatuo.HuatuoApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                    Debug.Log($"Ԫ���ݲ���:{ta.name}  ���:{err}");
#else
                    Debug.Log($"����Ԫ���ݲ����ļ�:{ta.name}");
#endif
                }
            }
        }
    }
}