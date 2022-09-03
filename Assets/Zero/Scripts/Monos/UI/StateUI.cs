using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// UI状态工具，主要是通过不同的状态值来控制GameObject的Activie
    /// </summary>
    public class StateUI : MonoBehaviour
    {


#if UNITY_EDITOR        
        [LabelText("默认状态"),OnValueChanged("OnSetDefaultStateInEditor")]
        public int defaultState;

        void OnSetDefaultStateInEditor()
        {
            SetState(defaultState);
        }
#endif


        /// <summary>
        /// 当前状态
        /// </summary>
        public int State { get; private set; }

        [LabelText("状态对应的GameObject列表(索引值表示状态值)")]
        public GameObject[] gameObjectList;

        int _state = -1;

        /// <summary>
        /// 如果State不存在，则所有的UI状态关联的GameObject都不会显示
        /// </summary>
        /// <param name="value"></param>
        public void SetState(int value)
        {
            if(value < 0 || value >= gameObjectList.Length)
            {
                value = -1;
            }
            _state = value;
            Refresh();
        }

        void Refresh()
        {
            for (int i = 0; i < gameObjectList.Length; i++)
            {
                gameObjectList[i].SetActive(i == _state ? true : false);
            }
        }
    }
}
