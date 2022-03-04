using System;
using System.Collections.Generic;

namespace Notice.Data.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class OrderAttribute : Attribute
    {
        public int Order { set; get; }

        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}
