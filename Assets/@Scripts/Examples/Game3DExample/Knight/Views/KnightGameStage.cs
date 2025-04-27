using UnityEngine;
using Zero;
using Zero;

namespace Knight
{
    class KnightGameStage : AView
    {
        public Camera StageCamera { get; private set; }

        Vector2 _move;
        Vector2 _sign;
        Vector3 _c2p;
        Vector3 _moveDir;
        Knight _knight;
        public Knight Knight
        {
            get
            {
                return _knight;            
            }
        }
        CameraController _cameraController;

        BlackKnight _blackKnight;


        protected override void OnInit(object data)
        {
            StageCamera = GetChildComponent<Camera>("Camera");
            _cameraController = GetChildComponent<CameraController>("Camera");            
            _knight = CreateChildView<Knight>("Knight");
            _blackKnight = CreateChildView<BlackKnight>("BlackKnight");
        }

        protected override void OnDestroy()
        {

        }

        protected override void OnEnable()
        {
            ILBridge.Ins.onUpdate += OnUpdate;
        }

        protected override void OnDisable()
        {
            ILBridge.Ins.onUpdate -= OnUpdate;
        }

        public void SetSign(Vector2 value)
        {
            _sign = value;
            _cameraController.rotate(new Vector3(value.x, value.y, 0));
        }

        public void SetMove(Vector2 value)
        {
            _move = value;
        }

        private void OnUpdate()
        {
            _blackKnight.canvas?.transform.LookAt(StageCamera.transform);
            ReviseDirByCamera();
        }

        /// <summary>
        /// 根据摄像机的朝向来修正相机方向
        /// </summary>
        void ReviseDirByCamera()
        {
            //人物和摄像机之间的向量
            _c2p = _knight.gameObject.transform.position - StageCamera.transform.position;
            _c2p.y = 0;
            //计算出这个向量和正前方的旋转角度
            Quaternion q = Quaternion.FromToRotation(Vector3.forward, _c2p);
            _moveDir = new Vector3(_move.x, 0, _move.y);
            //将移动方向沿着这个角度旋转得到最终人物真正移动的向量
            _c2p = Quaternion.AngleAxis(q.eulerAngles.y, Vector3.up) * _moveDir;
            _knight.Move(_c2p);
        }
    }
}
