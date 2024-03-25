using Jing;
using Jing.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;

namespace Example
{
    /// <summary>
    /// KCP协议库使用例子
    /// PS：此用例不含网络通信
    /// </summary>
    class KcpExample
    {
        public static void Start()
        {
            UIWinMgr.Ins.Open<KcpExampleWin>();
        }
    }

    class KcpExampleWin : WithCloseButtonWin
    {
        public InputField textInputBytesSize;
        public Button btnSend;
        public Button btnManualUpdate;
        public Button btnCleanLog;
        public Toggle toggleUpdateEnable;
        public Toggle toggleReceiveEnable;
        public Toggle toggleLogKcpDataEnable;

        public Text textKcpSettings;

        public Text textSendLog;
        public Text textReceiveLog;

        KCPHelper _a = new KCPHelper();
        KCPHelper _b = new KCPHelper();        

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            _a.onToSend += A2B;
            _a.onReceived += OnReceivedBytesA;

            _b.onToSend += B2A;
            _b.onReceived += OnReceivedBytesB;

            textInputBytesSize.text = _a.MSS.ToString();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("KCP配置：");
            sb.AppendLine($"mtu:{_a.settings.mtu}");
            sb.AppendLine($"mss:{_a.MSS}");
            sb.AppendLine($"sndwnd/rcvwnd:{_a.settings.sndwnd}");
            sb.AppendLine($"nodelay:{_a.settings.nodelay}");
            sb.AppendLine($"interval:{_a.settings.interval}");
            sb.AppendLine($"resend:{_a.settings.resend}");
            sb.AppendLine($"nc:{_a.settings.nc}");
            sb.AppendLine("丢包率：50%");
            textKcpSettings.text = sb.ToString();            
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            btnSend.onClick.AddListener(Send);
            btnManualUpdate.onClick.AddListener(ManualUpdate);
            btnCleanLog.onClick.AddListener(CleanLog);

            StartCoroutine(Update());

            //ILBridge.Ins.onUpdate += OnUpdate;
        }



        protected override void OnDisable()
        {
            base.OnDisable();

            btnSend.onClick.RemoveListener(Send);
            btnManualUpdate.onClick.RemoveListener(ManualUpdate);
            btnCleanLog.onClick.RemoveListener(CleanLog);

            StopAllCoroutines();

            //ILBridge.Ins.onUpdate -= OnUpdate;
        }

        private void CleanLog()
        {
            textSendLog.text = "发送端日志";
            textReceiveLog.text = "接收端日志";
        }

        private void ManualUpdate()
        {
            _a.Update();
            _b.Update();
        }


        DateTime _sendTime;
        private void Send()
        {
            _sendTime = DateTime.Now;

            string content = textInputBytesSize.text;
            var bytes = new byte[int.Parse(content)];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 7;
            }
            
            L(textSendLog, $"发送业务数据 size:{bytes.Length}");            
            _a.Send(bytes);
        }

        //private void OnUpdate()
        //{
        //    if (toggleUpdateEnable.isOn)
        //    {
        //        ManualUpdate();
        //    }
        //}

        IEnumerator Update()
        {
            while (true)
            {
                if (toggleUpdateEnable.isOn)
                {
                    ManualUpdate();
                }
                yield return null;
            }
        }

        private void OnReceivedBytesA(byte[] bytes)
        {
            L(textSendLog, $"收到业务数据 size:{bytes.Length}");
        }

        private void A2B(byte[] bytes)
        {
            if (toggleLogKcpDataEnable.isOn)
            {
                L(textSendLog, $"发送KCP数据 size:{bytes.Length}");
            }

            if (toggleReceiveEnable.isOn)
            {
                if (toggleLogKcpDataEnable.isOn)
                {
                    L(textReceiveLog, $"收到KCP数据 size:{bytes.Length}");
                }

                var k = UnityEngine.Random.Range(1, 10);
                if(k > 5)
                {
                    _b.KcpInput(bytes);
                }
                else
                {
                    if (toggleLogKcpDataEnable.isOn)
                    {
                        L(textSendLog, $"模拟丢包");
                    }
                }
                
            }
        }

        private void OnReceivedBytesB(byte[] bytes)
        {
            L(textReceiveLog, $"收到业务数据 size:{bytes.Length}");
        }

        private void B2A(byte[] bytes)
        {
            if (toggleLogKcpDataEnable.isOn)
            {
                L(textReceiveLog, $"发送KCP数据 size:{bytes.Length}");
                if(bytes.Length == KCP.IKCP_OVERHEAD)
                {
                    L(textSendLog, $"收到KCP ACK确认包");
                }
                else
                {
                    L(textSendLog, $"收到KCP数据 size:{bytes.Length}");
                }               
            }
            _a.KcpInput(bytes);
        }

        void L(Text text, string content)
        {
            //text.text += $"\r\n[{DateTime.Now.ToFileTimeUtc()}] {content}";
            //text.text += $"\r\n[{DateTime.Now.ToString("HH:mm:ss.fff")}] {content}";
            var tn = DateTime.Now - _sendTime;
            text.text += $"\r\n[延迟{(int)tn.TotalMilliseconds}ms] {content}";
        }
    }
}
