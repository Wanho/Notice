using System;

namespace Test.Core
{
	internal sealed class MatchValue : Code<MatchValue>
	{
		public readonly static MatchValue Null;

		[DefaultCode]
		public readonly static MatchValue Empty;

		static MatchValue()
		{
		}

		public MatchValue()
		{
		}
	}
}