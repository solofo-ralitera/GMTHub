using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTHub.Utils
{
    public static class StringUtils
    {
        public static string Reverse(string text)
        {
            if (text == null) return null;

            // this was posted by petebob as well 
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new String(array);
        }

        public static float ParseFloat(string text)
        {
            float value;
            Single.TryParse(
                text.Replace(',', '.'),
                NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out value
            );
            return value;
        }

        public static string[] SplitSize(string str, int n)
        {
            return Enumerable.Range(0, str.Length / n)
                            .Select(i => str.Substring(i * n, n)).ToArray();
        }
    }
}
