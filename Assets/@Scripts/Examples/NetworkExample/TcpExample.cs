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
using ZeroHot;

namespace Example
{
    class TcpExample
    {
        public const int PORT = 9527;

        public static void Start()
        {
            UIWinMgr.Ins.Open<TcpExampleWin>();
        }

        
    }

    class TcpExampleWin: WithCloseButtonWin
    {
        TcpExampleClientControlView clientView;
        TcpExampleServerControlView serverView;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            clientView = CreateChildView<TcpExampleClientControlView>("Client");
            serverView = CreateChildView<TcpExampleServerControlView>("Server");
        }
    }

    #region Client

    class TcpExampleClientControlView : AView
    {
        public InputField textInput;
        public Button btnConnect;
        public Button btnSend;
        public Text textLog;

        TcpClient client;

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

            btnConnect.onClick.AddListener(Connect);
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

            btnConnect.onClick.RemoveListener(Connect);
            btnSend.onClick.RemoveListener(Send);

            StopAllCoroutines();
        }

        private void Send()
        {
            var msg = textInput.text.Trim();

            if(msg == string.Empty)
            {
                L("发送内容不能为空!");
                return;
            }

            if(null == client || false == client.IsConnected)
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
                client = new TcpClient();
                client.onConnectSuccess += OnConnectSuccess;
                client.onConnectFail += OnConnectFail;
                client.onReceivedData += OnReceivedData;
                client.onDisconnect += OnDisconnect;
                client.Connect("127.0.0.1", TcpExample.PORT, 4096);
            }
            else
            {
                L("重连服务器...");
                client.Reconnect();
            }
        }

        private void OnConnectSuccess(TcpClient client)
        {
            L("服务器连接成功!");
            RefreshUI();
        }

        private void OnConnectFail(TcpClient client)
        {
            L("服务器连接失败!");
            RefreshUI();
        }

        private void OnReceivedData(TcpClient client, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            var msg = ba.ReadString();
            L(Zero.LogColor.Zero2(msg));
        }

        private void OnDisconnect(TcpClient client)
        {
            L("服务器连接断开!");
            RefreshUI();
        }

        void RefreshUI()
        {
            btnConnect.gameObject.SetActive(null == client || false == client.IsConnected);
            btnSend.gameObject.SetActive(null != client && client.IsConnected);
        }
    }

#endregion

    #region Server

    class TcpExampleServerControlView : AView
    {        
        public Button btnStart;
        public Button btnStop;
        public Text textLog;

        TcpServer server;

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
            if(null == server)
            {
                L("启动服务....");
                server = new TcpServer();
                server.onClientEnter += OnClientEnter;
                server.onClientExit += OnClientExit;                
                server.Start(TcpExample.PORT, 4096);
                
            }
            RefreshButton();
        }

        private void OnClientExit(IChannel obj)
        {
            L("客户端断开链接....");
            obj.onReceivedData -= OnReceivedData;
        }

        private void OnClientEnter(IChannel obj)
        {            
            L("客户端链接....");
            obj.onReceivedData += OnReceivedData;
        }

        private void OnReceivedData(IChannel sender, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            var msg = ba.ReadString();
            L($"收到消息:{Zero.LogColor.Zero2(msg)}");            

            ba.Reset();
            ba.Write($"服务器收到消息:{msg}");
            sender.Send(ba.GetAvailableBytes());            
        }

        private void StopServer()
        {
            L("停止服务....");
            if(server != null)
            {
                server.onClientEnter -= OnClientEnter;
                server.onClientExit -= OnClientExit;
                server.Close();
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
