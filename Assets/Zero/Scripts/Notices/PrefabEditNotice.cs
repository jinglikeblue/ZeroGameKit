using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zero
{
    /// <summary>
    /// Prefab编辑相关的通知
    /// </summary>
    public class PrefabEditNotice : ASingleton<PrefabEditNotice>
    {
        /// <summary>
        /// Preload的DLL执行方式改变
        /// </summary>
        public Action<EILType> onILTypeChanged;

        public override void Destroy()
        {
            
        }

        protected override void Init()
        {
            
        }
    }
}
