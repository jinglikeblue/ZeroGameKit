using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Knight
{
    class KnightLoadingPanel : AView
    {
        Image _bar;
        LoadingVO _vo;
        float _progress = 0f;
        protected override void OnInit(object data)
        {
            _bar = GetChildComponent<Image>("LoadingBar/Bar");
            _bar.fillAmount = 0;

            _vo = data as LoadingVO;
            StartCoroutine(SwitchDelay());
        }

        IEnumerator SwitchDelay()
        {
            yield return new WaitForSeconds(0.5f);            
            UIPanelMgr.Ins.SwitchAsync(_vo.switchType, _vo.switchData, null, onProgress);
        }

        private void onProgress(float progress)
        {
            _progress = progress;
        }

        protected override void OnEnable()
        {
            ILBridge.Ins.onUpdate += OnUpdate;
        }

        protected override void OnDisable()
        {
            ILBridge.Ins.onUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            if(_bar.fillAmount < _progress)
            {
                _bar.fillAmount += 0.1f;
                if(_bar.fillAmount > _progress)
                {
                    _bar.fillAmount = _progress;
                }
            }
        }
    }
}
