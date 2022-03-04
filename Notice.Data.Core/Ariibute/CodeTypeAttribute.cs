using System;
using System.Collections.Generic;

namespace Notice.Data.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CodeTypeAttribute : Attribute
    {
        public Type CodeType { set; get; }

        public CodeTypeAttribute(Type type)
        {
            CodeType = type;
        }
    }
}
