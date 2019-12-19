namespace System
{
	public class Sequence : IComparable
	{
		public readonly static Sequence Empty;

		private string @value;

		public string Value
		{
			get
			{
				return this.@value;
			}
		}

		static Sequence()
		{
			Sequence.Empty = new Sequence();
		}

		internal Sequence()
		{
			this.@value = string.Empty;
		}

		internal Sequence(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException();
			}
			this.@value = value.Trim();
		}

		public int CompareTo(object obj)
		{
			return this.@value.CompareTo(obj.ToString());
		}

		public override bool Equals(object obj)
		{
			return this.@value.Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			return this.@value.GetHashCode();
		}

		public static bool operator ==(Sequence sequence1, Sequence sequence2)
		{
			object empty;
			object obj;
			if (sequence1 != null)
			{
				empty = sequence1.@value;
			}
			else
			{
				empty = null;
			}
			if (empty == null)
			{
				empty = string.Empty;
			}
			if (sequence2 != null)
			{
				obj = sequence2.@value;
			}
			else
			{
				obj = null;
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			string str = (string)obj;
			return (string)empty == str;
		}

		public static explicit operator Sequence(string value)
		{
			if (value == null)
			{
				Sequence sequence = new Sequence();
			}
			return new Sequence(value);
		}

		public static implicit operator String(Sequence sequence)
		{
			if (sequence == null)
			{
				return null;
			}
			return sequence.Value;
		}

		public static bool operator !=(Sequence sequence1, Sequence sequence2)
		{
			object empty;
			object obj;
			if (sequence1 != null)
			{
				empty = sequence1.@value;
			}
			else
			{
				empty = null;
			}
			if (empty == null)
			{
				empty = string.Empty;
			}
			if (sequence2 != null)
			{
				obj = sequence2.@value;
			}
			else
			{
				obj = null;
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			string str = (string)obj;
			return (string)empty != str;
		}

		public override string ToString()
		{
			return this.@value;
		}
	}
}