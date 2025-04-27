using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using Zero;

namespace ZeroGameKit
{
    /// <summary>
    /// 菜单按钮组
    /// </summary>
    class MenuButtonGroupItem : AView
    {
        Text textGroupLabel;
        GameObject buttonPrefab;
        Transform content;
        
        List<GameObject> _buttonList = new List<GameObject>();


        protected override void OnInit(object data)
        {
            base.OnInit(data);
            buttonPrefab.SetActive(false);
            textGroupLabel.text = data as string;
            content = this.GetChild("ButtonList");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// 创建按钮
        /// </summary>
        /// <param name="label"></param>
        /// <param name="action"></param>
        public void AddBtn(string label, Action action)
        {
            var go = GameObject.Instantiate(buttonPrefab, content);
            if (null == action)
            {
                go.transform.SetAsLastSibling();
            }
            go.name = label;
            go.SetActive(true);
            go.GetComponentInChildren<Text>().text = label;
            if (action == null)
            {
                go.GetComponent<StateImage>().SetState(1);
            }
            else
            {
                go.GetComponent<StateImage>().SetState(0);
                go.GetComponent<Button>().onClick.AddListener(()=> {
                    action.Invoke();
                });
            }

            _buttonList.Add(go);
        }

        public void SwitchButtonShow(bool isShowTodo)
        {
            bool isaAnyButtonShowing = false;

            foreach(var go in _buttonList)
            {
                if(go.GetComponent<StateImage>().State == 1)
                {
                    go.SetActive(isShowTodo);
                }
                else
                {
                    isaAnyButtonShowing = true;
                }
            }

            this.SetActive(isaAnyButtonShowing || isShowTodo);
        }
    }
}
