using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtension
    {
        public static string CutTail(this string text, int tailLength)
        {
            if (text.Length >= tailLength) {
                return text.Substring(0, text.Length - tailLength);
            }

            return "";
        }

        public static string CutTail(this string text) 
        {
            return CutTail(text, 1);
        }

        public static int ToInt(this string text, int defautvalue)
        {
            try
            {
                return int.Parse(text);
            }
            catch
            {
                return defautvalue;
            }
        }

        public static string ToPascalCase(this string str)
        {
            string[] info = str.Split('_');
            string name = string.Empty;

            foreach (var w in info)
            {
                if (w.Length <= 1) {
                    name += w;
                }
                else {
                    name += w[0].ToString().ToUpper() + w.Substring(1).ToLower();
                }
            }
            return name;
        }
    }

    public static class ObjectExtension
    {
        public static string ToMoney(this object obj)
        {
            if (obj is string)
            {
                string val = obj as string;
                if (val == "0") return "0";
                return int.Parse(val).ToString("#,#");
            }
            else if (obj is int)
            {
                int val = (int)obj;
                if (val == 0) return "0";
                return val.ToString("#,#");
            }
            return "0";
        }

        public static string ToDate(this object obj, string format = "yyyy-MM-dd")
        {
            if (obj is string)
            {
                try
                {
                    if (string.IsNullOrEmpty(format)) {
                        return DateTime.Parse((string)obj).ToString("yyyy-MM-dd");
                    }
                    else {
                        return DateTime.Parse((string)obj).ToString(format);
                    }
                }
                catch { }
            }
            else if (obj is DateTime)
            {
                if (((DateTime)obj) > new DateTime(1900, 1, 1)) {
                    return ((DateTime)obj).ToString(format);
                }
            }

            return string.Empty;
        }

        public static DateTime ToDateTime(this string str)
        {
            DateTime returnValue;

            if (!DateTime.TryParse(str, out returnValue)) {
                returnValue = DateTime.MinValue;
            }

            return returnValue;
        }

        public static DateTime ToDateTime(this string str, DateTime defaultValue)
        {
            DateTime returnValue;

            if (!DateTime.TryParse(str, out returnValue)) {
                returnValue = defaultValue;
            }

            return returnValue;
        }

        public static bool ToBool(this object obj, bool defaultvalue = false)
        {
            try
            {
                return bool.Parse(obj.ToString());
            }
            catch
            {
                return defaultvalue;
            }
        }
    }
}
