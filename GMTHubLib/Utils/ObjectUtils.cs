using IniParser.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTHubLib.Utils
{
    public class ObjectUtils
    {
        public static void CopyValues<T>(T source, T target)
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var sourceValue = prop.GetValue(source, null);
                var targetValue = prop.GetValue(target, null);
                if (sourceValue != null && targetValue == null)
                {
                    prop.SetValue(target, sourceValue, null);
                }
            }
        }
    }
}
