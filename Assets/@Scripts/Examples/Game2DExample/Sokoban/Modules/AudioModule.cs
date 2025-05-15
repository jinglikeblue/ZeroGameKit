using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zero;
using ZeroGameKit;
using Zero;

namespace Sokoban
{
    public class AudioModule : BaseModule
    {
        public readonly AudioDevice bgm;
        public readonly AudioDevice effect;


        public AudioModule()
        {
            bgm = AudioDevice.Create("sokoban_bgm");
            effect = AudioDevice.Create("sokoban_effect");
            bgm.Play(ResMgr.Load<AudioClip>(R.sokoban_bgm_mp3));
        }

        public override void Dispose()
        {
            AudioDevice.Destroy(bgm);
            AudioDevice.Destroy(effect);            
        }
    }
}
