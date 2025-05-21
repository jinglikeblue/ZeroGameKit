using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

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

            //测试大小写兼容
            TestUpperAndLowerCase();
            
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
            img0.sprite = Assets.Load<Sprite>(AB.EXAMPLES_TEXTURES.NAME, AB.EXAMPLES_TEXTURES.activity_blue_ore_png);

            //通过资源完整路径加载
            img1.sprite = Assets.Load<Sprite>(R.activity_yellow_ore_01_png);
        }

        /// <summary>
        /// 异步加载方式
        /// </summary>
        async void AsyncLoad()
        {
            //通过指定AB和资源名加载
            var obj1 = await Assets.LoadAsync<Sprite>(AB.EXAMPLES_TEXTURES.NAME, AB.EXAMPLES_TEXTURES.gift_pudding_png,
                (sprite) =>
                {
                    img2.sprite = sprite;
                    L(LogColor.Zero2($"[加载完成][Onloaded] {sprite.name}"));
                },
                (progress) => { L(LogColor.Zero1($"{R.gift_pudding_png} 加载进度:{progress}")); }
            );

            L(LogColor.Zero2($"[加载完成][await] {obj1.name}"));

            //通过资源完整路径加载
            var obj2 = await Assets.LoadAsync<Sprite>(R.gift_sundae_png,
                (sprite) =>
                {
                    img3.sprite = sprite;
                    L(LogColor.Zero2($"[加载完成][Onloaded] {sprite.name}"));
                },
                (progress) => { L(LogColor.Zero1($"{R.gift_sundae_png} 加载进度:{progress}")); }
            );

            L(LogColor.Zero2($"[加载完成][await] {obj2.name}"));
        }

        /// <summary>
        /// 交叉依赖的加载
        /// </summary>
        void TestCrossDepend()
        {
            var a = Assets.Load<GameObject>(R.A_prefab);
            if (null != a)
            {
                L($"资源读取成功：{AB.EXAMPLES_CROSS_DEPEND_TEST_A.NAME}");
            }
            else
            {
                L($"资源读取失败：{AB.EXAMPLES_CROSS_DEPEND_TEST_A.NAME}");
            }

            var b = Assets.Load<GameObject>(R.B_prefab);
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

            L(LogColor.Zero1($"获取AB中的资源名称列表:{abName}"));
            var names = Assets.GetAllAsssetsNames(abName);
            for (int i = 0; i < names.Length; i++)
            {
                L(LogColor.Zero1($"{i}:{names[i]}"));
                var temp = Assets.Load(abName, names[i]);
            }

            L(LogColor.Zero1($"开始同步加载:{abName}"));
            var objs = Assets.LoadAll(abName);
            L(LogColor.Zero1($"加载完成，资源数:{objs?.Length}"));

            L(LogColor.Zero1($"开始异步加载:{abName}"));
            Assets.LoadAllAsync(abName, (assets) => { L(LogColor.Zero1($"加载完成，资源数:{assets?.Length}")); },
                (progress) => { L(LogColor.Zero1($"加载进度:{progress}")); });
        }

        /// <summary>
        /// 测试大小写兼容
        /// </summary>
        void TestUpperAndLowerCase()
        {
            var assetPath = "Test/test 1/Test 1.json";
            
            L(LogColor.Zero1($"======================= 大小写兼容测试"));
            L(LogColor.Zero1($"加载资源: {assetPath}"));
            
            var obj = Assets.Load(assetPath);

            if (null != obj)
            {
                L(LogColor.Green($"资源加载成功"));
            }
            else
            {
                L(LogColor.Red($"资源加载失败"));
            }
            
            L(LogColor.Zero1($"======================="));
        }
    }
}