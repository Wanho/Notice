using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public interface ICode { string Value { get; set; } }
    public interface INumericCode { int Value { get; set; } }

    [NotInitType]
    public abstract class Code<T> : BaseCode<T>, ICode where T : Code<T>
    {
        public string Value { get; set; }

        protected Code() { }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static Code<T> FindCode(object value)
        {
            string str = value as string;
            return BaseCode<T>.Codes.Find((T p) => p.Value.Equals(str, StringComparison.OrdinalIgnoreCase));
        }

        public static Code<T> FindCode_(string str)
        {
            return BaseCode<T>.Codes.Find((T p) => p.Value.Equals(str, StringComparison.OrdinalIgnoreCase));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Code<T> code, string str)
        {
            string value;
            if ((object)code != null)
            {
                value = code.Value;
            }
            else
            {
                value = null;
            }
            return value == str;
        }

        public static explicit operator Code<T>(string value)
        {
            return BaseCode<T>.Codes.Find((T p) => p.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public static implicit operator String(Code<T> code)
        {
            if ((object)code == null)
            {
                return null;
            }
            return code.Value;
        }

        public static bool operator !=(Code<T> code, string str)
        {
            string value;
            if ((object)code != null)
            {
                value = code.Value;
            }
            else
            {
                value = null;
            }
            return value != str;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
