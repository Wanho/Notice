using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ModelLoaderAttribute : Attribute
	{
		public static bool forceNoTimer;

		public int Interval
		{
			get;
			private set;
		}

		public int TryCount
		{
			get;
			private set;
		}

		static ModelLoaderAttribute()
		{
		}

		public ModelLoaderAttribute(int interval = 0, int tryCount = 5)
		{
			this.Interval = interval;
			this.TryCount = tryCount;
		}
	}
}