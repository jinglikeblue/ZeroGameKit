using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Sokoban
{
    public class LevelSelectWin : WithCloseButtonWin
    {
        Transform _list;
        GameObject _itemPrefab;        

        protected override void OnInit(object data)
        {
            _list = GetChild("Panel/LevelList/Viewport/Content");
            _itemPrefab = _list.Find("LevelItem").gameObject;
            _itemPrefab.SetActive(false);
            RefreshList();
        }

        void RefreshList()
        {
            //清除列表
            for (int i = 1; i < _list.childCount; i++)
            {
                GameObject.Destroy(_list.GetChild(i).gameObject);
            }

            for(int i = 0; i < Define.LEVEL_AMOUNT; i++)
            {
                ViewFactory.Create<ItemView>(_itemPrefab, _list, i + 1).SetActive(true);
            }
        }

        class ItemView : AView
        {
            int _level = -1;

            protected override void OnInit(object data)
            {
                base.OnInit(data);

                _level = (int)data;
                GetChildComponent<BitmapText>("Image/TextLevel").Text = _level.ToString();
            }

            protected override void OnEnable()
            {
                GetChildComponent<Button>("Image").onClick.AddListener(SelectLevel);
            }

            private void SelectLevel()
            {
                SokobanGlobal.Ins.Level.EnterLevel(_level);
            }
        }
    }
}
