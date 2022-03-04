using System;

namespace Notice.Web
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class HideAttribute : Attribute
    {
        public HideAttribute()
        {

        }
    }
}
