using Jing.TurbochargedScrollList;
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
    class GridScrollListDemoPanel : BaseScrollListDemoPanel
    {
        public TurbochargedGridScrollList listComponent;

        protected override void InitItems()
        {
            listComponent = scrollView.GetComponent<TurbochargedGridScrollList>();

            var datas = new int[itemCount];
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i] = i;
            }

            list = listComponent.GetList();
            list.onRenderItem += OnItemRender;
            list.onRebuildContent += OnRebuildContent;
            list.onRefresh += OnListRefresh;
            list.AddRange(datas);
        }

        private void OnItemRender(ScrollListItem item, object data, bool isRefresh)
        {
            if (isRefresh)
            {
                TurbochargedScrollListDemoItemView listItem;

                var zeroView = item.GetComponent<ZeroView>();
                if (null != zeroView)
                {
                    listItem = (TurbochargedScrollListDemoItemView)zeroView.aViewObject;
                }
                else
                {
                    listItem = CreateChildView<TurbochargedScrollListDemoItemView>(item.gameObject);
                }
                var listRT = list.scrollRect.GetComponent<RectTransform>();
                var content = string.Format("Index:{0}\nData:{1}", item.index, item.data);
                listItem.Refresh(150 + ((item.index % 15) * 20), listRT.rect.height - 40, content);
            }
        }
    }
}
