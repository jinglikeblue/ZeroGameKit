using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zero
{
    /// <summary>
    /// HuaTuo热更框架
    /// </summary>
    public class HuaTuoILWorker : AssemblyILWorker
    {
        public HuaTuoILWorker(Assembly assembly) : base(assembly)
        {
            //元数据补充
            HuaTuoAotMetadata.InitAotMetadata();
        }
    }
}
