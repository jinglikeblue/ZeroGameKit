using Jing.TurbochargedScrollList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zero;

namespace Example
{
    public class TurbochargedScrollListDemoItemView : AView
    {
        public Text text;

        public void Refresh(float w, float h, string content)
        {
            var item = gameObject.GetComponent<ScrollListItem>();

            text = transform.Find("Text").GetComponent<Text>();
            text.text = content;

            var rt = GetComponent<RectTransform>();
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        }
    }
}