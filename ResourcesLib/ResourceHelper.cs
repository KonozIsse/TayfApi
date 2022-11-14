using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesLib
{
    public class ResourceHelper
    {
        public static string ReadResourceValue<T>(string key, CultureInfo cultrueInfo = null) where T : class
        {
            return ReadResourceValue(typeof(T), key, cultrueInfo);
        }

        public static string ReadResourceValue(Type type, string key, CultureInfo cultrueInfo = null)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var resManager = new ResourceManager(type.FullName, type.Assembly);
                var strResourveValue = cultrueInfo == null ? resManager.GetString(key) : resManager.GetString(key, cultrueInfo);
                return strResourveValue ?? key;
            }
            return string.Empty;
        }
    }
}
