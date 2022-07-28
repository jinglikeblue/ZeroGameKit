using System.Collections;
using UnityEngine;
using Zero;
using ZeroHot;

namespace Example
{
    public class ResLoadExample
    {
        public static void Start()
        {
            var ex = new ResLoadExample();
            //交叉依赖的加载
            ex.TestCrossDepend();

            //读取所有AB文件
            ex.TestLoadAllAB();
        }

        private void TestCrossDepend()
        {
            var a = ResMgr.Ins.Load<GameObject>(AB.EXAMPLES_CROSS_DEPEND_TEST_A.A_assetPath);
            if (null != a)
            {
                Debug.Log($"资源读取成功：{AB.EXAMPLES_CROSS_DEPEND_TEST_A.NAME}");
            }
            else
            {
                Debug.Log($"资源读取失败：{AB.EXAMPLES_CROSS_DEPEND_TEST_A.NAME}");
            }
            var b = ResMgr.Ins.Load<GameObject>(AB.EXAMPLES_CROSS_DEPEND_TEST_B.B_assetPath);
            if (null != b)
            {
                Debug.Log($"资源读取成功：{AB.EXAMPLES_CROSS_DEPEND_TEST_B.NAME}");
            }
            else
            {
                Debug.Log($"资源读取失败：{AB.EXAMPLES_CROSS_DEPEND_TEST_B.NAME}");
            }
        }

        /// <summary>
        /// 测试加载AB中所有的资源
        /// </summary>
        private void TestLoadAllAB()
        {
            var abName = AB.ROOT_ASSETS.NAME;

            Debug.Log(Log.Zero1($"获取AB中的资源名称列表:{abName}"));
            var names = ResMgr.Ins.GetAllAsssetsNames(abName);
            for (int i = 0; i < names.Length; i++)
            {
                Debug.Log(Log.Zero1($"{i}:{names[i]}"));
                var temp = ResMgr.Ins.Load(abName, names[i]);
            }

            Debug.Log(Log.Zero1($"开始同步加载:{abName}"));
            var objs = ResMgr.Ins.LoadAll(abName);
            Debug.Log(Log.Zero1($"加载完成，资源数:{objs?.Length}"));

            Debug.Log(Log.Zero1($"开始异步加载:{abName}"));
            ResMgr.Ins.LoadAllAsync(abName, (assets) =>
            {
                Debug.Log(Log.Zero1($"加载完成，资源数:{assets?.Length}"));
            },
            (progress) =>
            {
                Debug.Log(Log.Zero1($"加载进度:{progress}"));
            });
        }

    }
}
