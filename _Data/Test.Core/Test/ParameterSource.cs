using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public class ParameterSource
	{
		internal ColumnAttribute Column
		{
			get;
			set;
		}

		internal string Name
		{
			get;
			set;
		}

		internal object Value
		{
			get;
			set;
		}

		public ParameterSource()
		{
		}

		public override string ToString()
		{
			return string.Format("Name={0}, Value={1}", this.Name, this.Value);
		}
	}
}