using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    /// <summary>
    /// StreamingAssets中直接加载热更资源的功能研发
    /// </summary>
    class StreamingAssetsLoadFuture
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<StreamingAssetsLoadFutureWin>();
        }
    }

    class StreamingAssetsLoadFutureWin : WithCloseButtonWin
    {
        public Button btnLoadResJson;
        public Button btnLoadSettingJson;
        public Button btnLoadAssetBundle;
        public Button btnLoadDll;
        public Button btnLoadConfigs;

        public Button btnCheckExist;
        public Button btnCheckInexistence;

        public Text textLog;

        public void L(string v)
        {
            textLog.text += "\r\n" + v;
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            btnLoadResJson.onClick.AddListener(LoadResJson);
            btnLoadSettingJson.onClick.AddListener(LoadSettingJson);
            btnLoadAssetBundle.onClick.AddListener(LoadAssetBundle);
            btnLoadDll.onClick.AddListener(LoadDll);
            btnLoadConfigs.onClick.AddListener(LoadConfigs);

            btnCheckExist.onClick.AddListener(CheckExist);
            btnCheckInexistence.onClick.AddListener(CheckInexistence);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            btnLoadResJson.onClick.RemoveListener(LoadResJson);
            btnLoadSettingJson.onClick.RemoveListener(LoadSettingJson);
            btnLoadAssetBundle.onClick.RemoveListener(LoadAssetBundle);
            btnLoadDll.onClick.RemoveListener(LoadDll);
            btnLoadConfigs.onClick.RemoveListener(LoadConfigs);

            btnCheckExist.onClick.RemoveListener(CheckExist);
            btnCheckInexistence.onClick.RemoveListener(CheckInexistence);
        }

        private void CheckExist()
        {

            var b = StreamingAssetsUtility.CheckFileExist(FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_PATH, "build_info"));
            L($"StreamingAssets中是否存在文件[build_info]: {b}");
            //StreamingAssetsUtility.CheckFileExist(FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_PATH, "build_info"), (isExist) =>
            //{
            //    L($"StreamingAssets中是否存在文件[build_info]: {isExist}");
            //});
        }

        private void CheckInexistence()
        {
            StreamingAssetsUtility.CheckFileExist(FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_PATH, "build_info_test"), (isExist) =>
            {
                L($"StreamingAssets中是否存在文件[build_info_test]: {isExist}");
            });
        }

        private void LoadResJson()
        {
            L("加载 res.json");
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, "res.json");
            Load(path, true);
        }

        private void LoadSettingJson()
        {
            L("加载 setting.json");
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, "setting.json");
            Load(path, true);
        }

        private void LoadAssetBundle()
        {
            L("加载 AssetBundle");
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, "ab", "examples.ab");
            var ab = StreamingAssetsUtility.LoadAssetBundle(path);
            if (null != ab)
            {
                L($"加载成功: {path}");
                var objList = ab.LoadAllAssets();
                foreach (var obj in objList)
                {
                    L($"加载到AB资源: {obj.name}");
                }
            }
            else
            {
                L($"加载失败: {path}");
            }

            L("------------------------------------------------------------");
        }

        private void LoadDll()
        {
            L("加载 dll");
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, "dll", "scripts.dll");
            Load(path, false);
            path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, "dll", "scripts.pdb");
            Load(path, false);
        }

        private void LoadConfigs()
        {
            L("加载 configs");

            LoadAB(AB.CONFIGS_TESTS.NAME, AB.CONFIGS_TESTS.test_json);

            L("------------------------------------------------------------");
        }

        void LoadAB(string abName, string assetName)
        {
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, ZeroConst.AB_DIR_NAME, abName);
            var ab = StreamingAssetsUtility.LoadAssetBundle(path);
            if (null != ab)
            {
                L($"加载成功: {path}");
                var obj = ab.LoadAsset(assetName);
                L($"加载到AB资源: {obj.name}");
            }
            else
            {
                L($"加载失败: {path}");
            }
        }

        void Load(string path, bool isText)
        {
            L($"加载路径: {path}");
            if (isText)
            {
                //StreamingAssetsUtility.LoadText(path, OnTextLoaded);

                var text = StreamingAssetsUtility.LoadText(path);
                OnTextLoaded(path, text);
            }
            else
            {
                //StreamingAssetsUtility.LoadData(path, OnDataLoaded);

                var bytes = StreamingAssetsUtility.LoadData(path);
                OnDataLoaded(path, bytes);
            }
        }

        private void OnDataLoaded(string path, byte[] bytes)
        {
            if (null == bytes || bytes.Length == 0)
            {
                L($"加载失败：{path}");
                return;
            }

            L($"加载内容长度: {bytes.Length}");
            L("------------------------------------------------------------");
        }

        private void OnTextLoaded(string path, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                L($"加载失败: {path}");
                return;
            }

            L($"加载内容长度: {text.Length}");
            L("------------------------------------------------------------");
        }
    }
}
