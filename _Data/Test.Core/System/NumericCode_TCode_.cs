using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
	[NotInitType]
	public abstract class NumericCode<TCode> : BaseCode<TCode>, INumericCode
	where TCode : NumericCode<TCode>
	{
		public int Value
		{
			get;
			set;
		}

		protected NumericCode()
		{
		}

		public override bool Equals(object obj)
		{
			return this.Value.Equals(obj);
		}

		public static NumericCode<TCode> FindCode(object value)
		{
			int num = (int)value;
			return BaseCode<TCode>.Codes.Find((TCode p) => p.Value == num);
		}

		public static NumericCode<TCode> FindCode_(int num)
		{
			return BaseCode<TCode>.Codes.Find((TCode p) => p.Value == num);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public static bool operator ==(NumericCode<TCode> code, int num)
		{
			if (code == null)
			{
				return false;
			}
			return code.Value == num;
		}

		public static explicit operator NumericCode<TCode>(int value)
		{
            //return (object)BaseCode<TCode>.Codes.Find((TCode p) => p.Value == value);
            return BaseCode<TCode>.Codes.Find((TCode p) => p.Value == value);
        }

		public static implicit operator Int32(NumericCode<TCode> code)
		{
			if (code == null)
			{
				return 0;
			}
			return code.Value;
		}

		public static bool operator !=(NumericCode<TCode> code, int num)
		{
			if (code == null)
			{
				return true;
			}
			return code.Value != num;
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}