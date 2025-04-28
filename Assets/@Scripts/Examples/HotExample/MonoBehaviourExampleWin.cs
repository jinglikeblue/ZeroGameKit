using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;

namespace Examples
{
    public class MonoBehaviourExampleWin : WithCloseButtonWin
    {
        public Text text;
        
        [BindingButtonClick("BtnAdd")]
        void Add()
        {
            var behaviour = this.gameObject.AddComponent<ExampleBehaviour>();
            behaviour.onUpdate += Update;
        }

        [BindingButtonClick("BtnRemove")]
        void Remove()
        {
            var behaviour = gameObject.GetComponent<ExampleBehaviour>();
            if (null != behaviour)
            {
                GameObject.DestroyImmediate(behaviour);    
            }
        }

        private void Update()
        {
            text.text = DateTime.Now.ToString();
        }
    }
}