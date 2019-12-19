using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public class ValueSet
	{
		public string DataSetName
		{
			get;
			set;
		}

		public ValueTableCollection Tables { get; } = new ValueTableCollection();

		public ValueSet()
		{
		}

		public ValueSet(string dataSetName)
		{
		}
	}
}