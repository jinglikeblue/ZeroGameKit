using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    class ResMgrExample
    {
        static public void Start()
        {            
            UIWinMgr.Ins.Open<ResMgrExampleWin>();
        }
    }

    class ResMgrExampleWin : WithCloseButtonWin
    {
        public Image img0;
        public Image img1;
        public Image img2;
        public Image img3;

        public Text textLog;



        protected override void OnInit(object data)
        {
            base.OnInit(data);

            SyncLoad();
            AsyncLoad();

            //交叉依赖的加载
            TestCrossDepend();

            //读取所有AB文件
            TestLoadAllAB();
        }

        public void L(string v)
        {
            textLog.text += "\r\n" + v;
        }


        /// <summary>
        /// 同步加载方式
        /// </summary>
        void SyncLoad()
        {
            //通过指定AB和资源名加载
            img0.sprite = ResMgr.Ins.Load<Sprite>(AB.EXAMPLES_TEXTURES.NAME, AB.EXAMPLES_TEXTURES.activity_blue_ore_png);

            //通过资源完整路径加载
            img1.sprite = ResMgr.Ins.Load<Sprite>(AB.EXAMPLES_TEXTURES.activity_yellow_ore_01_png_assetPath);
        }

        /// <summary>
        /// 异步加载方式
        /// </summary>
        void AsyncLoad()
        {
            //通过指定AB和资源名加载
            ResMgr.Ins.LoadAsync<Sprite>(AB.EXAMPLES_TEXTURES.NAME, AB.EXAMPLES_TEXTURES.gift_pudding_png,
                (sprite) =>
                {
                    img2.sprite = sprite;
                },
                (progress) =>
                {
                    L(Log.Zero1($"{AB.EXAMPLES_TEXTURES.gift_pudding_png_assetPath} 加载进度:{progress}"));
                }
                );            

            //通过资源完整路径加载
            ResMgr.Ins.LoadAsync<Sprite>(AB.EXAMPLES_TEXTURES.gift_sundae_png_assetPath,
                (sprite) =>
                {
                    img3.sprite = sprite;
                },
                (progress) =>
                {
                    L(Log.Zero1($"{AB.EXAMPLES_TEXTURES.gift_sundae_png_assetPath} 加载进度:{progress}"));
                }
                );
        }

        /// <summary>
        /// 交叉依赖的加载
        /// </summary>
        void TestCrossDepend()
        {
            var a = ResMgr.Ins.Load<GameObject>(AB.EXAMPLES_CROSS_DEPEND_TEST_A.A_assetPath);
            if (null != a)
            {
                L($"资源读取成功：{AB.EXAMPLES_CROSS_DEPEND_TEST_A.NAME}");
            }
            else
            {
                L($"资源读取失败：{AB.EXAMPLES_CROSS_DEPEND_TEST_A.NAME}");
            }
            var b = ResMgr.Ins.Load<GameObject>(AB.EXAMPLES_CROSS_DEPEND_TEST_B.B_assetPath);
            if (null != b)
            {
                L($"资源读取成功：{AB.EXAMPLES_CROSS_DEPEND_TEST_B.NAME}");
            }
            else
            {
                L($"资源读取失败：{AB.EXAMPLES_CROSS_DEPEND_TEST_B.NAME}");
            }
        }


        /// <summary>
        /// 测试加载AB中所有的资源
        /// </summary>
        void TestLoadAllAB()
        {
            var abName = AB.ROOT_ASSETS.NAME;

            L(Log.Zero1($"获取AB中的资源名称列表:{abName}"));
            var names = ResMgr.Ins.GetAllAsssetsNames(abName);
            for (int i = 0; i < names.Length; i++)
            {
                L(Log.Zero1($"{i}:{names[i]}"));
                var temp = ResMgr.Ins.Load(abName, names[i]);
            }

            L(Log.Zero1($"开始同步加载:{abName}"));
            var objs = ResMgr.Ins.LoadAll(abName);
            L(Log.Zero1($"加载完成，资源数:{objs?.Length}"));

            L(Log.Zero1($"开始异步加载:{abName}"));
            ResMgr.Ins.LoadAllAsync(abName, (assets) =>
            {
                L(Log.Zero1($"加载完成，资源数:{assets?.Length}"));
            },
            (progress) =>
            {
                L(Log.Zero1($"加载进度:{progress}"));
            });
        }
    }
}
