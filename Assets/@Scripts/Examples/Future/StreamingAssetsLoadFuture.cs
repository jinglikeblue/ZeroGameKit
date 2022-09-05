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
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            btnLoadResJson.onClick.RemoveListener(LoadResJson);
            btnLoadSettingJson.onClick.RemoveListener(LoadSettingJson);
            btnLoadAssetBundle.onClick.RemoveListener(LoadAssetBundle);
            btnLoadDll.onClick.RemoveListener(LoadDll);
            btnLoadConfigs.onClick.RemoveListener(LoadConfigs);
        }

        private void LoadResJson()
        {
            L("加载 res.json");
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, "res.json");
            this.StartCoroutine(Load(path));
        }

        private void LoadSettingJson()
        {
            L("加载 setting.json");
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, "setting.json");
            this.StartCoroutine(Load(path));
        }

        private void LoadAssetBundle()
        {
            L("加载 AssetBundle");
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, "ab", "examples.ab");

            var ab = AssetBundle.LoadFromFile(path);
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
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, "dll", "scripts.dll");
            this.StartCoroutine(Load(path));
            path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, "dll", "scripts.pdb");
            this.StartCoroutine(Load(path));
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
            var ab = AssetBundle.LoadFromFile(path);
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

        IEnumerator Load(string path)
        {
            L($"加载路径: {path}");
            var uwr = UnityWebRequest.Get(path);
            uwr.SendWebRequest();
            while (!uwr.isDone)
            {
                yield return 0;
            }

            if (uwr.error != null)
            {
                L(uwr.error);
                yield break;
            }

            var content = uwr.downloadHandler.text;
            Debug.Log(content);
            L($"加载内容长度: {uwr.downloadedBytes}");
            L("------------------------------------------------------------");
        }
    }
}
