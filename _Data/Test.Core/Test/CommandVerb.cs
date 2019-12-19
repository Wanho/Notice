using System;

namespace Test.Core
{
	internal sealed class CommandVerb : Code<CommandVerb>
	{
		public readonly static CommandVerb Delete;

		public readonly static CommandVerb Get;

		public readonly static CommandVerb Put;

		public readonly static CommandVerb Post;

		public readonly static CommandVerb Patch;

		[DefaultCode]
		public readonly static CommandVerb Auto;

		static CommandVerb()
		{
		}

		public CommandVerb()
		{
		}
	}
}