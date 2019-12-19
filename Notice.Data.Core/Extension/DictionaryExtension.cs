using System.Collections.Generic;

namespace Notice.Data.Core
{
    public static class DictionaryExtension
    {
        public static TValue SafeGet<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            TValue val = default(TValue);
            if (key == null) return val;

            dic.TryGetValue(key, out val);
            return val;
        }
    }
}
