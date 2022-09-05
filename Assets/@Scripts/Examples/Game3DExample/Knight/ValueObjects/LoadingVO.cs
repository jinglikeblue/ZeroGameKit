using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight
{
    class LoadingVO
    {
        public Type switchType;
        public object switchData;

        public LoadingVO(Type type, object data = null)
        {
            switchType = type;
            switchData = data;
        }
    }
}
