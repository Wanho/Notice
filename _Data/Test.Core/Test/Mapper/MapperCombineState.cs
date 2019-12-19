using System;

namespace Test.Core
{
	internal sealed class MapperCombineState : Code<MapperCombineState>
	{
		[DefaultCode]
		public readonly static MapperCombineState Ready;

		public readonly static MapperCombineState Complete;

		public readonly static MapperCombineState Dispose;

		static MapperCombineState()
		{
		}

		public MapperCombineState()
		{
		}
	}
}