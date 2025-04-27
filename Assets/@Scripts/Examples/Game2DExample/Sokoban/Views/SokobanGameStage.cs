using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Sokoban
{
    class SokobanGameStage : AView
    {
        Camera _camera;
        LevelModel _lv;
        Transform _contents;
        List<BaseUnit> _unitList;
        RoleUnit _roleUnit;
        EDir _lastMove;
        /// <summary>
        /// 操作记录
        /// </summary>
        Stack<RecordVO> _recordStack = new Stack<RecordVO>();

        protected override void OnInit(object data)
        {
            _contents = GetChild("Contents");
            _camera = GetChildComponent<Camera>("Camera");
            
            float off = Define.MAP_TILE_COUNT_OF_SIDE * Define.TILE_SIZE / -2f;
            _contents.localPosition = new Vector2(off, off);

            _unitList = new List<BaseUnit>();

            _lv = SokobanGlobal.Ins.Level.lv;
            CreateTargets();
            CreateBlocks();
            CreateBoxes();
            CreateRole();

            AdjustmentCamera();
        }

        protected override void OnEnable()
        {            
            ILBridge.Ins.onUpdate += OnUpdate;
            _roleUnit.onMoveEnd += OnRoleMoveEnd;
            SokobanGlobal.Ins.Notice.onScreenSizeChange += AdjustmentCamera;
        }

        protected override void OnDisable()
        {
            ILBridge.Ins.onUpdate -= OnUpdate;
            _roleUnit.onMoveEnd -= OnRoleMoveEnd;
            SokobanGlobal.Ins.Notice.onScreenSizeChange -= AdjustmentCamera;
        }

        /// <summary>
        /// 根据屏幕尺寸计算合适的相机大小
        /// </summary>
        private void AdjustmentCamera()
        {
            //如果不是竖屏则不用处理
            if(Screen.height <= Screen.width)
            {
                _camera.orthographicSize = 4.4f;
                return;
            }

            float size = (float)Screen.height / Screen.width * 4.4f;
            _camera.orthographicSize = size;

        }

        private void OnRoleMoveEnd(MoveableUnit obj)
        {
            MoveRole(_lastMove);
        }

        private void OnUpdate()
        {
            //进行排序
            DepthSort();
        }

        void DepthSort()
        {
            var st = new SortTool<BaseUnit>();
            foreach (var ub in _unitList)
            {
                st.AddItem((int)(ub.SortValue * 100), ub);
            }
            var sortList = st.Sort(true);
            for (int i = 0; i < sortList.Length; i++)
            {
                sortList[i].SetSortValue(i);
            }
        }

        private void CreateTargets()
        {
            var prefab = ResMgr.Load<GameObject>(AB.EXAMPLES_SOKOBAN_PREFABS_GAME.Target_assetPath);
            foreach (var vo in _lv.targets)
            {
                var unit = ViewFactory.Create<BaseUnit>(prefab, _contents, EUnitType.TARGET);
                unit.SetTile(vo.x, vo.y);                
            }
        }

        private void CreateBlocks()
        {
            var prefab = ResMgr.Load<GameObject>(AB.EXAMPLES_SOKOBAN_PREFABS_GAME.Block_assetPath);
            foreach (var vo in _lv.blocks)
            {
                var unit = ViewFactory.Create<BaseUnit>(prefab, _contents, EUnitType.BLOCK);
                unit.SetTile(vo.x, vo.y);
                _unitList.Add(unit);
            }
        }

        private void CreateBoxes()
        {
            var prefab = ResMgr.Load<GameObject>(AB.EXAMPLES_SOKOBAN_PREFABS_GAME.Box_assetPath);
            foreach (var vo in _lv.boxes)
            {
                var unit = ViewFactory.Create<BoxUnit>(prefab, _contents, EUnitType.BOX);
                unit.SetTile(vo.x, vo.y);
                _unitList.Add(unit);
            }
        }

        private void CreateRole()
        {
            var prefab = ResMgr.Load<GameObject>(AB.EXAMPLES_SOKOBAN_PREFABS_GAME.Role_assetPath);
            foreach (var vo in _lv.roles)
            {
                var unit = ViewFactory.Create<RoleUnit>(prefab, _contents, EUnitType.ROLE);
                unit.SetTile(vo.x, vo.y);
                _unitList.Add(unit);
                _roleUnit = unit;
            }
        }

        public bool MoveRole(EDir dir)
        {
            _lastMove = dir;

            if (EDir.NONE == dir)
            {
                return false;
            }

            if(_roleUnit.IsMoving)
            {
                return false;
            }

            var endTile = GetTileMoveTo(_roleUnit.Tile, dir);

            //检查目标位置是否有阻挡
            var block = GetUnitInTile(endTile);
            if(null != block)
            {
                if(EUnitType.BLOCK == block.UnitType)
                {
                    return false;
                }

                if(EUnitType.BOX == block.UnitType)
                {
                    //推动箱子
                    var moveBoxSuccess = MoveBox(block as BoxUnit, dir);
                    if(false == moveBoxSuccess)
                    {
                        //箱子推动失败
                        return false;
                    }
                }
            }

            return _roleUnit.Move(dir, endTile);
        }

        bool MoveBox(BoxUnit box, EDir dir)
        {
            if(null == box)
            {
                return false;
            }

            var endTile = GetTileMoveTo(box.Tile, dir);
            //检查目标位置是否有阻挡
            var block = GetUnitInTile(endTile);
            if (null != block && (EUnitType.BLOCK == block.UnitType || EUnitType.BOX == block.UnitType))
            {
                return false;
            }

            _recordStack.Push(new RecordVO(_roleUnit.Tile, box.Tile, dir, _unitList.IndexOf(box)));
            box.onMoveEnd += OnBoxMoveEnd;
            return box.Move(dir, endTile);
        }

        private void OnBoxMoveEnd(MoveableUnit unit)
        {
            unit.onMoveEnd -= OnBoxMoveEnd;

            if(_lv.IsTarget((ushort)unit.Tile.x, (ushort)unit.Tile.y))
            {
                (unit as BoxUnit).SetIsAtTarget(true);
                //播放一个效果
                ViewFactory.Create<BangEffect>(AB.EXAMPLES_SOKOBAN_PREFABS_GAME.NAME, AB.EXAMPLES_SOKOBAN_PREFABS_GAME.BangEffect, unit.gameObject.transform);
                CheckLevelComplete();
            }
            else
            {
                (unit as BoxUnit).SetIsAtTarget(false);
            }
        }

        void CheckLevelComplete()
        {
            foreach (var unit in _unitList)
            {
                if (unit.UnitType == EUnitType.BOX)
                {
                    if (false == _lv.IsTarget((ushort)unit.Tile.x, (ushort)unit.Tile.y))
                    {
                        return;
                    }
                }
            }

            SokobanMsgWin.Show("Congratulations!", false, () => {
                //通知关卡完成
                SokobanGlobal.Ins.Notice.onLevelComplete?.Invoke();
            }).SetLabel("Next");            
        }

        /// <summary>
        /// 得到格子上的单位
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        BaseUnit GetUnitInTile(Vector2Int tile)
        {
            foreach(var unit in _unitList)
            {
                if(unit.Tile == tile)
                {
                    return unit;
                }
            }
            return null;
        }

        Vector2Int GetTileMoveTo(Vector2Int startTile, EDir dir)
        {
            Vector2Int endTile = startTile;
            switch (dir)
            {
                case EDir.UP:
                    endTile = startTile + Vector2Int.up;
                    break;
                case EDir.DOWN:
                    endTile = startTile + Vector2Int.down;
                    break;
                case EDir.LEFT:
                    endTile = startTile + Vector2Int.left;
                    break;
                case EDir.RIGHT:
                    endTile = startTile + Vector2Int.right;
                    break;
            }
            return endTile;
        }

        public void Revoke()
        {
            if (_recordStack.Count > 0)
            {
                var vo = _recordStack.Pop();
                _roleUnit.SetTile((ushort)vo.roleTile.x, (ushort)vo.roleTile.y);
                _roleUnit.SetToward(vo.dir);
                var box = _unitList[vo.boxIdx];
                box.SetTile((ushort)vo.boxTile.x, (ushort)vo.boxTile.y);


                if (_lv.IsTarget((ushort)box.Tile.x, (ushort)box.Tile.y))
                {
                    (box as BoxUnit).SetIsAtTarget(true);
                }
                else
                {
                    (box as BoxUnit).SetIsAtTarget(false);
                }
            }
        }
    }
}
