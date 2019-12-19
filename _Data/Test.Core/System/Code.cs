using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
	[NotInitType]
	public abstract class Code<TCode> : BaseCode<TCode>, ICode
	where TCode : Code<TCode>
	{
		public string Value
		{
			get;
			set;
		}

		protected Code()
		{
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public static Code<TCode> FindCode(object value)
		{
			string str = value as string;
			return BaseCode<TCode>.Codes.Find((TCode p) => p.Value.Equals(str, StringComparison.OrdinalIgnoreCase));
		}

		public static Code<TCode> FindCode_(string str)
		{
			return BaseCode<TCode>.Codes.Find((TCode p) => p.Value.Equals(str, StringComparison.OrdinalIgnoreCase));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		//public static bool operator ==(Code<TCode> code, string str)
		//{
		//	string value;
		//	if (code != null)
		//	{
		//		value = code.Value;
		//	}
		//	else
		//	{
		//		value = null;
		//	}
		//	return value == str;
		//}

		public static explicit operator Code<TCode>(string value)
		{
			return BaseCode<TCode>.Codes.Find((TCode p) => p.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
		}

		public static implicit operator String(Code<TCode> code)
		{
			if (code == null)
			{
				return null;
			}
			return code.Value;
		}
        
		//public static bool operator !=(Code<TCode> code, string str)
		//{
		//	string value;
		//	if (code != null)
		//	{
		//		value = code.Value;
		//	}
		//	else
		//	{
		//		value = null;
		//	}
		//	return value != str;
		//}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CodeAttribute : Attribute
    {
        public string[] Group
        {
            get;
            private set;
        }

        public int NumericValue
        {
            get;
            private set;
        }

        public int Order
        {
            get;
            private set;
        }

        public string Value
        {
            get;
            private set;
        }

        public CodeAttribute(string value = null, int order = -1, params string[] group)
        {
            this.Value = value;
            this.Order = order;
            this.Group = group;
        }

        public CodeAttribute(int value, int order = -1, params string[] group)
        {
            this.NumericValue = value;
            this.Order = order;
            this.Group = group;
        }

        public CodeAttribute(string[] group)
        {
            this.Group = group;
            this.Order = -1;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class CodeDataAttribute : Attribute
    {
        public string Name
        {
            get;
            private set;
        }

        public string Value
        {
            get;
            private set;
        }

        public CodeDataAttribute(string name, string value)
        {
            this.Value = value;
            this.Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CodeGroupAttribute : Attribute
    {
        public string[] Group
        {
            get;
            private set;
        }

        public CodeGroupAttribute(params string[] group)
        {
            this.Group = group;
        }
    }
}