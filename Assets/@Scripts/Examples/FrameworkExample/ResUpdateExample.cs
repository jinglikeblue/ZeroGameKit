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
    class ResUpdateExample
    {
        static public void Start()
        {            
            UIWinMgr.Ins.Open<ResUpdateExampleWin>();
        } 
    }

    class ResUpdateExampleWin : WithCloseButtonWin
    {
        /// <summary>
        /// 路径地址为热更资源[平台]目录下的相对路径
        /// <para>查找以name字符串为开头的资源，格式可以为 "ab/h.ab" 或 "dll/" </para>
        /// <para>如果没有以"."或"/"结尾，则会自动查超所有符合[name加上"."或"/"结尾]的文件</para>
        /// <para>输入 "" 或者 "/" 则会返回所有的资源</para>
        /// </summary>
        readonly string[] updateList = new string[] {
                $"ab/{AB.EXAMPLES_AUDIOS.NAME}",  //下载AB
                $"ab/{AB.EXAMPLES_TEXTURES.NAME}", //下载AB
                "dll/" //下载dll文件夹中的所有资源
            };

        public Button btnUpdate;

        public Text textLog;

        HotResUpdater updater;

        private bool isUseTask = true;

        public void L(string v)
        {
            textLog.text += "\r\n" + v;
        }

        void CleanLog()
        {
            textLog.text = string.Empty;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            btnUpdate.onClick.AddListener(OnBtnUpdateClick);
        }

        private async void OnBtnUpdateClick()
        {
            if (!Runtime.Ins.IsHotResEnable)
            {
                MsgWin.Show("提示", "请使用AssetBundle模式，并且打开热更资源开关再使用该用例");
                return;
            }

            if (null != updater)
            {
                MsgWin.Show("提示", "更新进行中...");
                return;
            }

            CleanLog();
            L("开始更新资源");
            updater = new HotResUpdater(updateList);
            //这次为了测试，强制进行所有资源的下载。
            updater.IsForceUpdateAll = true;

            if (isUseTask)
            {
                L($"方式1： 通过Task");
                await updater.StartAsync(OnUpdaterProgress);
                OnUpdaterComplete(updater);
            }
            else
            {
                L($"方式2： 通过回调");
                updater.onComplete += OnUpdaterComplete;
                updater.onProgress += OnUpdaterProgress;
                updater.Start();      
            }

            isUseTask = !isUseTask;

        }

        private void OnUpdaterComplete(BaseUpdater updater)
        {
            if(updater.error != null)
            {
                L($"[结束]Update 出错:{updater.error}");
            }
            else
            {
                L($"[结束]Update结束 完成");
            }

            this.updater = null;
        }

        private void OnUpdaterProgress(long loadedSize, long totalSize)
        {
            L($"[进行中]Update 进度:{loadedSize}   总长度:{totalSize}");
        }
    }

}
