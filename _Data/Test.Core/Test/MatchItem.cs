using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	internal abstract class MatchItem : ValueObject
	{
		public string Name { get; set; }
		public string Text { get; set; }
		public virtual string Type { get; }

		protected MatchItem() { }
	}

    internal class IncludeMatchItem : MatchItem, IValueItem
    {
        public string Property { get; set; }
        public override string Type { get; } = "Include";
        public string Value { get; set; }

        public IncludeMatchItem() { }
    }

    internal class ExistMatchItem : ExcludeMatchItem
    {
        public override string Type { get; } = "Exist";

        public ExistMatchItem() { base.Value = MatchValue.Empty.Value; }
    }

    internal class ExcludeMatchItem : MatchItem, IValueItem
    {
        public string Property { get; set; }
        public override string Type { get; } = "Exclude";
        public string Value { get; set; }

        public ExcludeMatchItem() { }
    }

    internal class EmptyMatchItem : IncludeMatchItem
    {
        public override string Type { get; } = "Empty";

        public EmptyMatchItem() { base.Value = MatchValue.Empty.Value; }
    }

    internal class ConditionMatchItem : MatchItem
    {
        public override string Type { get; } = "Condition";

        public ConditionMatchItem() { }
    }

    internal class AllMatchItem : MatchItem
    {
        public override string Type { get; } = "Empty";

        public AllMatchItem() { }
    }
}