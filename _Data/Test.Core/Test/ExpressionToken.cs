using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public class ExpressionToken
	{
		public bool? IsOperand;

		public string Name { get; set; } = string.Empty;

		public ExpressionOpCode OpCode
		{
			get;
			private set;
		}

		public ExpressionToken()
		{
		}

		public void SetCode()
		{
			this.OpCode = (ExpressionOpCode)((Code<ExpressionOpCode>)this.Name);
			if (this.OpCode == null)
			{
				this.OpCode = ExpressionOpCode.None;
			}
			this.IsOperand = new bool?(this.OpCode == ExpressionOpCode.None);
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}