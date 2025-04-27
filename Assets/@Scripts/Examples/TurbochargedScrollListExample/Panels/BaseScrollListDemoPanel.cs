using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;
using ZeroGameKit;
using Zero;

namespace Example
{
    abstract class BaseScrollListDemoPanel : AView
    {
        /// <summary>
        /// 默认Item数量
        /// </summary>
        public int itemCount = 100;

        public IScrollList list { get; protected set; }

        protected GameObject scrollView;

        public Button btnExit;
        public Button btnClear;
        public Button btnAdd;
        public InputField inputNumber;
        public Button btnInsert;
        public Button btnRemoveAt;
        public Button btnRemove;
        public Button btnScroll2Index;
        public Button btnScroll2End;

        /// <summary>
        /// 输入的数量
        /// </summary>
        protected int InputNumber
        {
            get
            {                
                int count = 0;
                int.TryParse(inputNumber.text, out count);
                return count;
            }
        }

        protected int GetInputValue()
        {            
            if (string.IsNullOrEmpty(inputNumber.text))
            {
                return 0;
            }
            return int.Parse(inputNumber.text);
        }

        protected override void OnInit(object data)
        {

            scrollView = GetChildGameObject("Scroll View");

            
            btnExit.onClick.AddListener(() =>
            {
                UIPanelMgr.Ins.Switch<ScrollListDemoMenuPanel>();
            });

            
            btnClear.onClick.AddListener(() =>
            {
                Clear();
            });

            
            
            btnAdd.onClick.AddListener(() =>
            {
                AddRange();
            });

            
            btnInsert.onClick.AddListener(() =>
            {
                Insert();
            });

            btnRemoveAt.onClick.AddListener(() =>
            {
                RemoveAt();
            });

            
            btnRemove.onClick.AddListener(() =>
            {
                Remove();
            });

            
            btnScroll2Index.onClick.AddListener(() =>
            {
                ScrollToItem();
            });

            
            btnScroll2End.onClick.AddListener(() =>
            {
                ScrollToPosition();
            });

            InitItems();
        }

        protected void AddRange()
        {
            var datas = new int[InputNumber];
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i] = Random.Range(1, 10000);
            }
            list.AddRange(datas);
        }

        protected void Clear()
        {
            list.Clear();
        }

        protected void Insert()
        {
            list.Insert(GetInputValue(), Random.Range(1, 10000));
        }

        protected void Remove()
        {
            list.Remove(GetInputValue());
        }

        protected void RemoveAt()
        {
            list.RemoveAt(GetInputValue());
        }

        protected void ScrollToItem()
        {
            list.ScrollToItem(GetInputValue());
        }

        protected void ScrollToPosition()
        {
            list.ScrollToPosition(new Vector2(0, list.ContentHeight));
        }

        protected abstract void InitItems();

        protected void OnListRefresh()
        {
            //Debug.Log("列表刷新");
        }

        protected void OnRebuildContent()
        {
            //Debug.Log("列表高度改变");
        }
    }
}
