﻿using Jing;
using Jing.Net;
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
using Zero;

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

    class UdpExampleCommon
    {
        public static string LocalIP
        {
            get
            {
                // 获取本机的主机名
                string hostName = System.Net.Dns.GetHostName();

                // 根据主机名获取本机的IP地址列表
                System.Net.IPAddress[] addresses = System.Net.Dns.GetHostAddresses(hostName);

                foreach (System.Net.IPAddress address in addresses)
                {
                    // 判断IP地址是否为IPv4地址以排除IPv6地址
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        // 输出IP地址
                        return address.ToString();
                    }
                }

                return null;
            }
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
        public InputField textInputIP;

        UdpClient client;

        private ThreadSyncActions _tsa = new ThreadSyncActions();

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            RefreshUI();
            textInputIP.text = UdpExampleCommon.LocalIP;
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

            StartCoroutine(SyncThreadActions());
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            //btnConnect.onClick.RemoveListener(Connect);
            btnSend.onClick.RemoveListener(Send);

            StopAllCoroutines();
        }

        IEnumerator SyncThreadActions()
        {
            while (true)
            {
                _tsa?.RunSyncActions();
                yield return null;
            }
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
                L($"连接服务器... {textInputIP.text}:{UdpExample.SERVER_PORT}  本地端口：{UdpExample.CLIENT_PORT}");
                client = new UdpClient();
                client.onReceivedData += OnReceivedData;
                client.Bind(textInputIP.text, UdpExample.SERVER_PORT, UdpExample.CLIENT_PORT, 4096);
            }
        }

        private void OnReceivedData(UdpClient client, byte[] data)
        {
            _tsa.AddToSyncAction(() =>
            {
                ByteArray ba = new ByteArray(data);
                var msg = ba.ReadString();
                L(Zero.LogColor.Zero2(msg));
            });
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

        private ThreadSyncActions _tsa = new ThreadSyncActions();

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

            StartCoroutine(SyncThreadActions());
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            btnStart.onClick.RemoveListener(StartServer);
            btnStop.onClick.RemoveListener(StopServer);

            StopAllCoroutines();
        }

        IEnumerator SyncThreadActions()
        {
            while (true)
            {
                _tsa?.RunSyncActions();
                yield return null;
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
                L($"启动服务.... {UdpExampleCommon.LocalIP}:{UdpExample.SERVER_PORT}");
                server = new UdpServer();
                server.onReceivedData += OnReceivedData;
                server.Bind(UdpExample.SERVER_PORT, 4096);
            }

            RefreshButton();
        }

        private void OnReceivedData(UdpServer server, EndPoint ep, byte[] data)
        {
            _tsa.AddToSyncAction(() =>
            {
                ByteArray ba = new ByteArray(data);
                var msg = ba.ReadString();
                L(Zero.LogColor.Zero2($"收到消息:{msg}"));

                ba.Reset();
                ba.Write($"服务器收到消息:{msg}");
                if (sendChannel == null)
                {
                    sendChannel = server.CreateSendChannel(ep);
                }

                sendChannel.Send(ba.GetAvailableBytes());
            });
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