using Jing.TurbochargedScrollList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Example
{
    class VerticalScrollListDemoPanel : BaseScrollListDemoPanel
    {        
        public GameObject itemPrefab;

        protected override void OnInit(object data)
        {           
            itemPrefab = GetComponent<GameObjectBindingData>().Find("listitem")[0];

            base.OnInit(data);
        }

        protected override void InitItems()
        {
            var datas = new int[itemCount];
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i] = i;
            }

            var layout = new VerticalLayoutSettings();
            layout.gap = 10;
            layout.paddingTop = 50;
            list = new VerticalScrollList(scrollView.GetComponent<ScrollRect>(), itemPrefab, layout);
            list.onRenderItem += OnItemRender;
            list.onRebuildContent += OnRebuildContent;
            list.onRefresh += OnListRefresh;
            list.AddRange(datas);
        }

        protected void OnItemRender(ScrollListItem item, object data, bool isRefresh)
        {
            if (isRefresh)
            {
                TurbochargedScrollListDemoItemView listItem;

                var zeroView = item.GetComponent<ZeroView>();
                if(null != zeroView)
                {
                    listItem = (TurbochargedScrollListDemoItemView)zeroView.aViewObject;
                }
                else
                {
                    listItem = CreateChildView<TurbochargedScrollListDemoItemView>(item.gameObject);
                }
                
                var listRT = list.scrollRect.GetComponent<RectTransform>();
                var content = string.Format("Index:{0} Data:{1}", item.index, item.data);
                listItem.Refresh(listRT.rect.width - 40, 100 + ((item.index % 15) * 20), content);
            }

            //Debug.LogFormat("渲染Item [idx:{0}, value:{1}]", item.index, data);
        }
    }
}
