using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zero;

namespace Example
{
    class BehaviacExample
    {
        static public void Start()
        {
            new BehaviacUsecase();
        }
    }

    class BehaviacUsecase
    {
        NewAgent _na;
        public BehaviacUsecase()
        {
            Debug.Log("InitBehavic");

            var cfm = new behaviac.CustomFileManager();
            behaviac.Workspace.Instance.FilePath = "./";
            behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_bson;

            _na = new NewAgent();
            var bRet = _na.btload("TestBehavior");
            if (bRet)
            {
                _na.btsetcurrent("TestBehavior");
            }


            ILBridge.Ins.onFixedUpdate += OnFixedUpdate;
        }

        private void OnFixedUpdate()
        {

            _na.btexec();
        }
    }
}
