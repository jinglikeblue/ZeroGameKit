using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;
using ZeroGameKit;

namespace Knight
{
    public class AudioModule : BaseModule
    {
        public AudioDevice Device { get; private set; }

        public AudioModule()
        {
            Device = AudioDevice.Create("music");
        }

        public override void Dispose()
        {
            AudioDevice.Destroy(Device);
            Device = null;
        }
    }
}
