using System;
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
    class CryptoExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<CryptoExampleWin>();
        }
    }

    /// <summary>
    /// MD5
    /// AES
    /// </summary>
    class CryptoExampleWin: WithCloseButtonWin
    {
        public Toggle toggleMD5;
        public Toggle toggleAES;

        public InputField textInput;
        public Button btnCrypto;
        public Text textOutput;

        protected override void OnEnable()
        {
            base.OnEnable();
            btnCrypto.onClick.AddListener(OnBtnCryptoClick);
        }

        private void OnBtnCryptoClick()
        {
            if (toggleMD5.isOn)
            {
                MD5Crypto();
            }

            if (toggleAES.isOn)
            {
                AESCrypto();
            }


        }

        void MD5Crypto()
        {
            var input = textInput.text.Trim();
            var sb = new StringBuilder();
            sb.AppendLine($"32 Size MD5: {CryptoUtility.Get32ByteMD5(input)}");
            sb.AppendLine($"16 Size MD5: {CryptoUtility.Get16ByteMD5(input)}");
            var output = sb.ToString();

            textOutput.text = output;
        }

        void AESCrypto()
        {
            var input = textInput.text.Trim();
            var sb = new StringBuilder();

            var cryptoStr = CryptoUtility.AESEncryptString(input, "ZeroGameKit");

            sb.AppendLine($"AES 加密: {cryptoStr}");
            sb.AppendLine($"AES 解密: {CryptoUtility.AESDecryptString(cryptoStr, "ZeroGameKit")}");
            var output = sb.ToString();

            textOutput.text = output;
        }
    }
}
