using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public class MapperProviderInfo
	{
		public string Catalog
		{
			get;
			set;
		}

		public string ConnectionString
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public List<ParameterSource> Parameter { get; protected set; } = new List<ParameterSource>();

		public string ProviderType
		{
			get;
			set;
		}

		public MapperProviderInfo()
		{
		}

		public void AddParameter(string name, object value)
		{
			this.Parameter.Add(new ParameterSource()
			{
				Name = name,
				Value = value
			});
		}
	}
}