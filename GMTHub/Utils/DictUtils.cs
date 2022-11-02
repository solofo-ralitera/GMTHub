using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTHub.Utils
{
    public static class DictUtils
    {
        public static string GetString(Dictionary<string, string> dict, string key, string defaultValue = "")
        {
            string res;
            if(dict.TryGetValue(key, out res))
            {
                return res.Trim();
            }
            return defaultValue;
        }
    }
}
