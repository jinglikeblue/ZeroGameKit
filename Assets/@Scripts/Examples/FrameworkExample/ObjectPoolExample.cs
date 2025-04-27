using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;
using ZeroGameKit;
using Zero;

namespace Example
{
    class ObjectPoolExample
    {
        public static StringBuilder sb = new StringBuilder();

        public static void Start()
        {
            sb.AppendLine("创建一个容量为3的对象池");
            var pool = ObjectPoolMgr.Ins.CreatePool<ObjectItem>("object_item_pool", 3);

            sb.AppendLine("创建5个对象，并尝试放入对象池");
            for (int i = 0; i < 5; i++)
            {
                var name = "Obj_" + i;
                var obj = new ObjectItem(name);
                pool.Recycle(obj);

                sb.AppendLine($"对象池当前容量: {pool.CurrentSize}/{pool.poolMaxSize}");
            }

            sb.AppendLine("尝试获取1个对象");
            var item = pool.GetInstance();
            sb.AppendLine($"获取到对象：{item.name} 对象池当前容量: {pool.CurrentSize}/{pool.poolMaxSize}");

            sb.AppendLine("清空对象池");
            ObjectPoolMgr.Ins.ClearPool("object_item_pool");
            sb.AppendLine($"对象池当前容量: {pool.CurrentSize}/{pool.poolMaxSize}");

            var msg = MsgWin.Show("ObjectPoolExample", sb.ToString());
            msg.SetContentAlignment(UnityEngine.TextAnchor.MiddleLeft);
        }

        class ObjectItem : IRecyclable
        {
            public string name;
            public ObjectItem(string name)
            {
                this.name = name;
            }

            public void OnDiscarded()
            {
                sb.AppendLine($"{LogColor.Zero2(name)}: 被对象池遗弃");
            }

            public void OnRecycled()
            {
                sb.AppendLine($"{LogColor.Zero2(name)}: 被回收到对象池");
            }
        }

    }
}
