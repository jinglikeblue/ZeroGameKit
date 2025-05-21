using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Example
{
    class AudioDeviceExample
    {
        public static void Start()
        {
            UIWinMgr.Ins.Open<AudioDeviceExampleWin>();
        }
    }


    class AudioDeviceExampleWin : WithCloseButtonWin
    {
        AudioDevice _bgmAD;
        AudioDevice _effectAD;        

        Slider sliderBGMVolume;
        Slider sliderEffectVolume;
        Button btnPlayEffect;
        Toggle toggleBGM;

        string[] audioEffects = new string[] {
            AB.EXAMPLES_AUDIOS.effect_0_mp3,
            AB.EXAMPLES_AUDIOS.effect_1_mp3,
            AB.EXAMPLES_AUDIOS.effect_2_mp3,
            AB.EXAMPLES_AUDIOS.effect_3_mp3,
            AB.EXAMPLES_AUDIOS.effect_4_mp3,
            AB.EXAMPLES_AUDIOS.effect_5_mp3,
            AB.EXAMPLES_AUDIOS.effect_6_mp3,
            AB.EXAMPLES_AUDIOS.effect_7_mp3,
            AB.EXAMPLES_AUDIOS.effect_8_mp3,
            AB.EXAMPLES_AUDIOS.effect_9_mp3,
        };

        int _effectIdx = -1;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            _bgmAD = AudioDevice.Get("BGM", true);

            _effectAD = AudioDevice.Get("EFFECT", true);

            //播放背景音乐
            _bgmAD.Play(Assets.Load<AudioClip>(R.examples_bgm_mp3), true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            AudioDevice.Destroy(_bgmAD);
            AudioDevice.Destroy(_effectAD);
        }

        void PlayNextEffect()
        {
            _effectIdx++;
            if(_effectIdx >= audioEffects.Length)
            {
                _effectIdx = 0; 
            }

            var ae = audioEffects[_effectIdx];
            var ac = Assets.Load<AudioClip>(AB.EXAMPLES_AUDIOS.NAME, ae);
            _effectAD.Play(ac);
        }

        protected override void OnEnable()
        {
            base.OnEnable();            
            btnPlayEffect.onClick.AddListener(OnBtnPlayEffectClick);

            sliderBGMVolume.onValueChanged.AddListener(OnBgmVolumeChange);
            sliderEffectVolume.onValueChanged.AddListener(OnEffectVolumeChange);

            toggleBGM.onValueChanged.AddListener(OnToggleBGMValueChange);
        }

        protected override void OnDisable()
        {
            base.OnDisable();            
            btnPlayEffect.onClick.RemoveListener(OnBtnPlayEffectClick);

            sliderBGMVolume.onValueChanged.RemoveListener(OnBgmVolumeChange);
            sliderEffectVolume.onValueChanged.RemoveListener(OnEffectVolumeChange);

            toggleBGM.onValueChanged.RemoveListener(OnToggleBGMValueChange);
        }

        private void OnToggleBGMValueChange(bool isOn)
        {
            if(isOn)
            {
                _bgmAD.UnpauseAll();
            }
            else
            {
                _bgmAD.PauseAll();
            }
            
        }

        private void OnBtnPlayEffectClick()
        {
            PlayNextEffect();
        }

        private void OnBgmVolumeChange(float volume)
        {
            _bgmAD.Volume = volume;
        }

        private void OnEffectVolumeChange(float volume)
        {
            _effectAD.Volume = volume;
        }
    }
}
