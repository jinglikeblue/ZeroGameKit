using GameKit;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Knight
{
    class KnightGamePanel : AView
    {
        KnightGameStage _stage;
        Joystick _moveJoystick;
        Touchpad _signTouchpad;
        Button _btnSetting;

        Button _btnAtk;
        Button _btnDef;

        protected override void OnInit(object data)
        {
            _btnSetting = GetChildComponent<Button>("BtnSetting");
            _stage = StageMgr.Ins.Switch<KnightGameStage>();
            _moveJoystick = GetChildComponent<Joystick>("Joystick");
            //_moveJoystick.uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            _signTouchpad = GetChildComponent<Touchpad>("Touchpad");

            _btnAtk = GetChildComponent<Button>("BtnAtk");
            _btnDef = GetChildComponent<Button>("BtnDef");
        }

        protected override void OnDestroy()
        {
            StageMgr.Ins.Clear();
        }

        protected override void OnEnable()
        {
            _btnSetting.onClick.AddListener(ShowSettingWin);
            _moveJoystick.onValueChange += OnMoveValueChange;
            _signTouchpad.onValueChange += OnSignValueChange;


            PointerDownEventListener.Get(_btnAtk.gameObject).onEvent += (e)=> {
                _stage.Knight.Attack(true);
            };

            PointerUpEventListener.Get(_btnAtk.gameObject).onEvent += (e) => {
                _stage.Knight.Attack(false);
            };

            PointerDownEventListener.Get(_btnDef.gameObject).onEvent += (e) => {
                _stage.Knight.Block(true);
            };

            PointerUpEventListener.Get(_btnDef.gameObject).onEvent += (e) => {
                _stage.Knight.Block(false);
            };

            G.Ins.Audio.Device.StopAll();
            G.Ins.Audio.Device.Play(ResMgr.Load<AudioClip>(AB.EXAMPLES_KNIGHT_AUDIOS.BattleBGM_mp3_assetPath), true);            
        }

        protected override void OnDisable()
        {
            _btnSetting.onClick.RemoveListener(ShowSettingWin);
            _moveJoystick.onValueChange -= OnMoveValueChange;
            _signTouchpad.onValueChange -= OnSignValueChange;            
        }

        private void ShowSettingWin()
        {
            UIWinMgr.Ins.Open<KnightSettingWin>();            
        }

        private void OnSignValueChange(Vector2 v)
        {
            _stage.SetSign(v);
        }

        private void OnMoveValueChange(Vector2 v)
        {
            _stage.SetMove(v);
        }
    }
}
