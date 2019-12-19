using System;
using System.Collections.Generic;
using System.Reflection;

namespace Test.Core
{
	public static class Initializer
	{
		public static void Run(IEnumerable<Assembly> assemblies, string mapperPath, long startSequence = -1L)
		{
			BaseCodeInitializer.Initialize(assemblies, startSequence);
			MapperProvider.Initialize(assemblies, mapperPath);
			ValueObject.Initialize(assemblies);
		}
	}
}