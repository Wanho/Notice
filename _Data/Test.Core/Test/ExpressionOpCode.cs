using System;

namespace Test.Core
{
	public class ExpressionOpCode : Code<ExpressionOpCode>
	{
		[Code("", 0, new string[] {  })]
		public readonly static ExpressionOpCode None;

		[Code("(", 1, new string[] {  })]
		public readonly static ExpressionOpCode Left;

		[Code(")", 1, new string[] {  })]
		public readonly static ExpressionOpCode Right;

		[Code("==", 2, new string[] {  })]
		public readonly static ExpressionOpCode Equal;

		[Code("!=", 2, new string[] {  })]
		public readonly static ExpressionOpCode NotEqual;

		[Code("<>", 2, new string[] {  })]
		public readonly static ExpressionOpCode NotEqual2;

		[Code(">=", 2, new string[] {  })]
		public readonly static ExpressionOpCode GreaterThanOrEqual;

		[Code("<=", 2, new string[] {  })]
		public readonly static ExpressionOpCode LessThanOrEqual;

		[Code(">", 2, new string[] {  })]
		public readonly static ExpressionOpCode GreaterThan;

		[Code("<", 2, new string[] {  })]
		public readonly static ExpressionOpCode LessThan;

		[Code("and", 3, new string[] {  })]
		public readonly static ExpressionOpCode And;

		[Code("or", 4, new string[] {  })]
		public readonly static ExpressionOpCode Or;

		public ExpressionOpCode()
		{
		}
	}
}