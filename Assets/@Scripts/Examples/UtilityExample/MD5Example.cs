using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGameKit;
using ZeroHot;
using Zero;
using UnityEngine.UI;

namespace Example
{
    class MD5Example
    {
        public static void Start()
        {
            UIWinMgr.Ins.Open<MD5ExampleWin>();
        }        
    }

    class MD5ExampleWin : WithCloseButtonWin
    {
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
            var input = textInput.text.Trim();
            var sb = new StringBuilder();
            sb.AppendLine($"32 Size MD5: {CryptoUtility.Get32ByteMD5(input)}");
            sb.AppendLine($"16 Size MD5: {CryptoUtility.Get16ByteMD5(input)}");
            var output = sb.ToString();

            textOutput.text = output;
        }
    }
}
