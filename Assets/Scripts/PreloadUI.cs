using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Zero;

namespace Demo
{
    public class PreloadUI : MonoBehaviour
    {
        public Text text;

        /// <summary>
        /// 离线运行
        /// </summary>
        public Toggle toggleOffLineMode;

        /// <summary>
        /// 使用dll
        /// </summary>
        public Toggle toggleUseDll;

        /// <summary>
        /// 开启热更
        /// </summary>
        public Toggle toggleHotPatchMode;

        /// <summary>
        /// 开启日志
        /// </summary>
        public Toggle toggleLog;

        public Text textNetRoots;

        private LauncherSettingData _data;

        private GameObject _runtimeSettings;

        private void Awake()
        {
            text.text = string.Empty;
            _runtimeSettings = transform.DeepFind("RuntimeSettings").gameObject;
            _runtimeSettings.SetActive(true);
            
            _data = LauncherSetting.LoadLauncherSettingDataFromResources();
            toggleUseDll.isOn = _data.isUseDll;
            toggleHotPatchMode.isOn = _data.isHotPatchEnable;
            toggleLog.isOn = _data.isLogEnable;
            toggleOffLineMode.isOn = _data.isOfflineEnable;
            RefreshUI();
        }

        private void Start()
        {
            var btnStartupT = TransformUtility.DeepFindChild(this.transform, "BtnStartup");
            var btnStartup = btnStartupT.GetComponent<Button>();
            btnStartup.onClick.AddListener(Startup);
        }

        private void OnEnable()
        {
            toggleHotPatchMode.onValueChanged.AddListener((isOn) => { RefreshUI(); });
        }

        void RefreshUI()
        {
            if (_data.isUseAssetBundle)
            {
                toggleHotPatchMode.gameObject.SetActive(true);
                toggleOffLineMode.gameObject.SetActive(toggleHotPatchMode.isOn);
                textNetRoots.gameObject.SetActive(toggleHotPatchMode.isOn);
                if (toggleHotPatchMode.isOn)
                {
                    StringBuilder sb = new StringBuilder("网络资源地址：");
                    for (int i = 0; i < _data.urlRoots.Length; i++)
                    {
                        sb.AppendLine();
                        sb.Append($"[{i}]: {_data.urlRoots[i]}");
                    }

                    textNetRoots.text = sb.ToString();
                }
                else
                {
                    textNetRoots.text = string.Empty;
                }
            }
            else
            {
                toggleHotPatchMode.gameObject.SetActive(false);
                toggleOffLineMode.gameObject.SetActive(false);
                textNetRoots.gameObject.SetActive(false);
            }
        }

        private async void Startup()
        {
            // try
            // {
                SetProgress(0, 1);

                var vo = LauncherSetting.LoadLauncherSettingDataFromResources();
                vo.isUseDll = toggleUseDll.isOn;
                vo.isHotPatchEnable = toggleHotPatchMode.isOn;
                vo.isLogEnable = toggleLog.isOn;
                vo.isOfflineEnable = toggleOffLineMode.isOn;
            
                //从这里启动Ppreload
                var launcher = new Launcher(vo);
                launcher.onProgress += SetProgress;
                launcher.onError += s => { text.text = $"Error: {s}"; };
                var error = await launcher.Start();
                if (string.IsNullOrEmpty(error))
                {
                    Destroy(gameObject);    
                }

                text.text = error;
            // }
            // catch (Exception e)
            // {
            //     Debug.LogError(e);
            //     text.text = "启动失败!";
            // }
        }

        private void SetProgress(long loadedSize, long totalSize)
        {
            if (0 == totalSize)
            {
                text.text = string.Empty;
                return;
            }
            
            float totalMB = totalSize / 1024 / 1024f;
            float loadedMB = loadedSize / 1024 / 1024f;
            var progress = (int)((float)loadedSize / totalSize * 100);
            //转换为MB
            text.text = string.Format("加载进度： {0}% [{1}MB/{2}MB]", progress, loadedMB.ToString("0.00"), totalMB.ToString("0.00"));
        }
    }
}