using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GMTHub.Utils
{
    public class Blinker
    {
        public Timer BlinkTimer;
        public bool TimerStatus = true;

        public Blinker()
        {
            BlinkTimer = new Timer(500);
            BlinkTimer.Elapsed += (sender, args) => {
                TimerStatus = !TimerStatus;
            };
            BlinkTimer.AutoReset = true;
            BlinkTimer.Enabled = true;
        }
    }
}
