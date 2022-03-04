using System;
using System.Collections.Generic;

namespace Notice.Data.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public sealed class CodeNameAttribute : Attribute
    {
        public string CodeNm { set; get; }

        public CodeNameAttribute(string name)
        {
            CodeNm = name;
        }

    }
}
