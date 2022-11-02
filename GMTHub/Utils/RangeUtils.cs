using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTHub.Utils
{
    public static class RangeUtils
    {
        // Re-maps a number from one range to another.
        public static float Remap(float val, float in1, float in2, float out1, float out2)
        {
            return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
        }
    }
}
