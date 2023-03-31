using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jing
{
    /// <summary>
    /// 通知中心
    /// </summary>
    public static class Notice
    {
        static Dictionary<string, List<NoticeEventDelegate>> _noticeMap = new Dictionary<string, List<NoticeEventDelegate>>();

        public delegate void NoticeEventDelegate(string noticeName, params object[] datas);        

        public static void Send(string noticeName, params object[] datas)
        {
            if(false == _noticeMap.ContainsKey(noticeName))
            {
                return;
            }

            var delegateList = _noticeMap[noticeName];
            foreach(var delegateAction in delegateList)
            {
                delegateAction?.Invoke(noticeName, datas);
            }
        }

        public static void AddListener(string noticeName, NoticeEventDelegate n)
        {
            if (false == _noticeMap.ContainsKey(noticeName))
            {
                _noticeMap.Add(noticeName, new List<NoticeEventDelegate>());
            }

            _noticeMap[noticeName].Add(n);
        }

        public static void RemoveListener(string noticeName, NoticeEventDelegate n)
        {
            if (false == _noticeMap.ContainsKey(noticeName))
            {
                return;
            }

            _noticeMap[noticeName].Remove(n);            
        }
    }
}
