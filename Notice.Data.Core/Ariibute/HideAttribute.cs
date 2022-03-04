using System;

namespace Notice.Data.Core
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class HideAttribute : Attribute
    {
        public HideAttribute()
        {

        }
    }
}
