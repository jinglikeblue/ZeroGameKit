using DG.Tweening;
using UnityEngine;

namespace Example
{
    public class DoTweenExample
    {
        public static void Start()
        {
            var ilcontent = GameObject.Find("MenuPanel");
            ilcontent.transform.DOShakePosition(0.5f, 5);
        }
    }
}