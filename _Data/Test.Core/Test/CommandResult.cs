using System;

namespace Test.Core
{
	public sealed class CommandResult : Code<CommandResult>
	{
		public readonly static CommandResult None;

		public readonly static CommandResult Scalar;

		public readonly static CommandResult Row;

		public readonly static CommandResult Table;

		public readonly static CommandResult Set;

		public readonly static CommandResult Output;

		static CommandResult()
		{
		}

		public CommandResult()
		{
		}
	}
}