using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;

namespace Demo
{
    public class PreloadUI : MonoBehaviour
    {

        public Text text;

        void Start()
        {    
            SetProgress(0, 1);

            var vo = LauncherSetting.LoadLauncherSettingDataFromResources();
            var launcher = new Launcher(vo);

            //Preload preload = GetComponent<Preload>();
            launcher.onProgress += SetProgress;

            launcher.onStateChange += (state) =>
            {                
                Debug.Log("Preload State Change: " + state);
                if (state == Launcher.EState.STARTUP)
                {
                    GameObject.Destroy(this.gameObject);
                }
            };

            //从这里启动Ppreload
            launcher.Start();            
        }

        private void SetProgress(long loadedSize, long totalSize)
        {
            var progress = (int)((float)loadedSize / totalSize * 100);
            //转换为MB
            text.text = $"{progress}%[{loadedSize}/{totalSize}]";
        }
    }
}