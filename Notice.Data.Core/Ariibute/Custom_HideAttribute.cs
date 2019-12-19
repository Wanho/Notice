using System;

namespace Notice.Data.Core
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class Custom_HideAttribute : Attribute
    {
        public Custom_HideAttribute()
        {

        }
    }
}
