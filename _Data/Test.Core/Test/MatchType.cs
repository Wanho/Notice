using System;

namespace Test.Core
{
	internal sealed class MatchType : Code<MatchType>
	{
		static MatchType() { }

		public MatchType() { }

		public MatchItem CreateMatchItem()
		{
			string name = base.Name;
			if (name == "All")
			{
				return new AllMatchItem();
			}
			if (name == "Empty")
			{
				return new EmptyMatchItem();
			}
			if (name == "Exist")
			{
				return new ExistMatchItem();
			}
			if (name == "Include")
			{
				return new IncludeMatchItem();
			}
			if (name == "Exclude")
			{
				return new ExcludeMatchItem();
			}
			if (name == "Condition")
			{
				return new ConditionMatchItem();
			}
			return null;
		}
	}
}