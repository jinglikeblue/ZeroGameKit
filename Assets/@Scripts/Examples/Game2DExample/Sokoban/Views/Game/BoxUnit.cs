﻿using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Sokoban
{
    /// <summary>
    /// 箱子
    /// </summary>
    class BoxUnit : MoveableUnit
    {
        public void SetIsAtTarget(bool isAtTarget)
        {            
            var sprites = GetComponent<ObjectBindingData>().Find("BoxState");
            if (isAtTarget)
            {
                Img.sprite = sprites[1] as Sprite;
            }
            else
            {
                Img.sprite = sprites[0] as Sprite;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            onMoveStart += OnMoveStart;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            onMoveStart -= OnMoveStart;
        }        

        private void OnMoveStart(MoveableUnit obj)
        {
            var ac = Assets.Load<AudioClip>(R.push_mp3);
            SokobanGlobal.Ins.Audio.effect.Play(ac);
        }
    }
}
