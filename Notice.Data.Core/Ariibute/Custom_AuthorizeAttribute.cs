using System;

namespace Notice.Data.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class Custom_AuthorizeAttribute : Attribute
    {
        public AuthType AuthType { set; get; }
        public string RedirectType { set; get; }

        public Custom_AuthorizeAttribute()
        {
            AuthType = AuthType.User;
        }

        public Custom_AuthorizeAttribute(string authType)
        {
            AuthType = (AuthType)authType;
        }
    }
}
