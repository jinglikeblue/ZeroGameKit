using One;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    class UdpExample
    {
        public const int SERVER_PORT = 9528;
        public const int CLIENT_PORT = 8528;

        public static void Start()
        {
            UIWinMgr.Ins.Open<UdpExampleWin>();
        }


    }

    class UdpExampleWin : WithCloseButtonWin
    {
        UdpExampleClientControlView clientView;
        UdpExampleServerControlView serverView;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            clientView = CreateChildView<UdpExampleClientControlView>("Client");
            serverView = CreateChildView<UdpExampleServerControlView>("Server");
        }
    }

    #region Client

    class UdpExampleClientControlView : AView
    {
        public InputField textInput;
        public Button btnSend;
        public Text textLog;

        UdpClient client;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            RefreshUI();
        }



        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        void L(string content)
        {
            textLog.text += "\r\n" + content;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            //btnConnect.onClick.AddListener(Connect);
            btnSend.onClick.AddListener(Send);

            StartCoroutine(Update());
        }

        IEnumerator Update()
        {
            while (true)
            {
                client?.Refresh();
                yield return new WaitForEndOfFrame();
            }
        }



        protected override void OnDisable()
        {
            base.OnDisable();

            //btnConnect.onClick.RemoveListener(Connect);
            btnSend.onClick.RemoveListener(Send);

            StopAllCoroutines();
        }

        private void Send()
        {
            Connect();

            var msg = textInput.text.Trim();

            if (msg == string.Empty)
            {
                L("发送内容不能为空!");
                return;
            }

            if (null == client)
            {
                L("服务器未连接!");
                return;
            }

            L("发送内容：" + msg);

            var ba = new ByteArray();
            ba.Write(msg);

            client?.Send(ba.GetAvailableBytes());
        }

        private void Connect()
        {
            if (null == client)
            {
                L("连接服务器...");
                client = new UdpClient();
                client.onReceiveData += OnReceiveData;
                client.Bind("127.0.0.1", UdpExample.SERVER_PORT, UdpExample.CLIENT_PORT, 4096);
            }
        }

        private void OnReceiveData(UdpClient client, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            var msg = ba.ReadString();
            L(Zero.Log.Zero2(msg));
        }

        void RefreshUI()
        {
            //btnConnect.gameObject.SetActive(null == client);
            //btnSend.gameObject.SetActive(null != client);
        }
    }

    #endregion

    #region Server

    class UdpExampleServerControlView : AView
    {
        public Button btnStart;
        public Button btnStop;
        public Text textLog;

        UdpServer server;
        UdpSendChannel sendChannel;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            RefreshButton();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            btnStart.onClick.AddListener(StartServer);
            btnStop.onClick.AddListener(StopServer);

            StartCoroutine(Update());
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            btnStart.onClick.RemoveListener(StartServer);
            btnStop.onClick.RemoveListener(StopServer);

            StopAllCoroutines();
        }

        IEnumerator Update()
        {
            while (true)
            {
                server?.Refresh();
                yield return new WaitForEndOfFrame();
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void StartServer()
        {
            if (null == server)
            {
                L("启动服务....");
                server = new UdpServer();
                server.onReceiveData += OnReceiveData;                
                server.Bind(UdpExample.SERVER_PORT, 4096);

            }
            RefreshButton();
        }

        private void OnReceiveData(UdpServer server, EndPoint ep, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            var msg = ba.ReadString();
            L(Zero.Log.Zero2($"收到消息:{msg}"));

            ba.Reset();
            ba.Write($"服务器收到消息:{msg}");
            if (sendChannel == null)
            {
                sendChannel = server.CreateSendChannel(ep);
            }
            sendChannel.Send(ba.GetAvailableBytes());
        }

        private void StopServer()
        {
            L("停止服务....");
            if (server != null)
            {
                server.Dispose();
                server = null;
            }
            RefreshButton();
        }

        void RefreshButton()
        {
            btnStop.gameObject.SetActive(null != server);
            btnStart.gameObject.SetActive(null == server);
        }

        void L(string content)
        {
            textLog.text += "\r\n" + content;
        }
    }
    #endregion
}
