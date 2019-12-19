using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public static class ValueSetExtension
	{
		internal static ValueRow FirstRow(this ValueSet ds)
		{
			if (ds.Tables.Count == 0)
			{
				return null;
			}
			return ds.Tables[0].FirstRow();
		}
	}
}