using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchService.Launcher
{
    public interface IMeetingLauncher
    {
        void Launch(string service);
    }
}
