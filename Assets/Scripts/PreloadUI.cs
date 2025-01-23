using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zero;

namespace Demo
{
    public class PreloadUI : MonoBehaviour
    {
        public Text text;
        public Toggle toggleOffLineMode;
        public Toggle toggleUseDll;

        LauncherSettingData data => LauncherSetting.LoadLauncherSettingDataFromResources();

        private void Awake()
        {
            toggleUseDll.isOn = data.isUseDll;
        }

        private void Start()
        {
            var btnStartupT = TransformUtility.DeepFindChild(this.transform, "BtnStartup");
            var btnStartup = btnStartupT.GetComponent<Button>();
            btnStartup.onClick.AddListener(Startup);
        }

        private void Startup()
        {
            SetProgress(0, 1);

            var vo = LauncherSetting.LoadLauncherSettingDataFromResources();
            vo.isUseDll = toggleUseDll.isOn;
            var launcher = new Launcher(vo, toggleOffLineMode.isOn);

            //Preload preload = GetComponent<Preload>();
            launcher.onProgress += SetProgress;

            launcher.onStateChange += (state) =>
            {
                Debug.Log("Preload State Change: " + state);
                if (state == Launcher.EState.STARTUP)
                {
                    var content = GameObject.Find("ILContent");
                    if (content != null)
                    {
                        Destroy(content);
                    }

                    GameObject.Destroy(this.gameObject);
                }
            };

            //从这里启动Ppreload
            launcher.Start();
        }

        private void SetProgress(long loadedSize, long totalSize)
        {
            float totalMB = totalSize / 1024 / 1024f;
            float loadedMB = loadedSize / 1024 / 1024f;
            var progress = (int)((float)loadedSize / totalSize * 100);
            //转换为MB
            text.text = string.Format("加载进度： {0}% [{1}MB/{2}MB]", (int)(progress * 100f), loadedMB.ToString("0.00"), totalMB.ToString("0.00"));
        }
    }
}