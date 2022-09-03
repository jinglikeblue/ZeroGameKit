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
            Preload preload = GetComponent<Preload>();
            preload.onProgress += SetProgress;

            preload.onStateChange += (state) =>
            {
                Debug.Log("Preload State Change: " + state);                
            };

            //从这里启动Ppreload
            preload.StartPreload();            
        }

        private void SetProgress(long loadedSize, long totalSize)
        {
            var progress = (int)((float)loadedSize / totalSize * 100);
            //转换为MB
            text.text = $"{progress}%[{loadedSize}/{totalSize}]";
        }
    }
}