using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Test.Core
{
	public abstract class MapperProvider
	{
		protected string catalog = string.Empty;

		protected string connectionString = string.Empty;

		internal virtual Regex DBParameterRegex { get; } = new Regex("N?(@([a-zA-Z0-9_]+))", RegexOptions.Compiled);

		public static MapperProvider DefaultProvider
		{
			get
			{
				return MapperProvider.CreateProvider(MapperProvider.DefaultProviderName);
			}
		}

		public static string DefaultProviderName
		{
			get;
			set;
		}

		public bool Logging { get; protected set; } = true;

		public static Dictionary<string, Test.Core.MapperProviderInfo> MapperProviderInfo
		{
			get;
			private set;
		}

		public static Dictionary<string, MapperProvider> MapperProviderList
		{
			get;
			private set;
		}

		public static Dictionary<string, Type> MapperProviderType
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			protected set;
		}

		internal List<ParameterSource> Parameter
		{
			get;
			set;
		}

		static MapperProvider()
		{
			MapperProvider.MapperProviderType = new Dictionary<string, Type>();
			MapperProvider.MapperProviderInfo = new Dictionary<string, Test.Core.MapperProviderInfo>();
			MapperProvider.MapperProviderList = new Dictionary<string, MapperProvider>();
		}

		protected MapperProvider()
		{
		}

		public static Test.Core.MapperProviderInfo Add(string name, string connectionString, string providerType)
		{
			if (!MapperProvider.MapperProviderType.ContainsKey(providerType))
			{
				throw new Exception(string.Concat(providerType, " not exist MapperProvider class"));
			}
			if (MapperProvider.MapperProviderInfo.ContainsKey(name))
			{
				return null;
			}
			Test.Core.MapperProviderInfo mapperProviderInfo = new Test.Core.MapperProviderInfo()
			{
				Name = name,
				ConnectionString = connectionString
			};
			string[] strArrays = connectionString.Split(new char[] { ';' });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (!string.IsNullOrEmpty(str))
				{
					string[] strArrays1 = str.Split(new char[] { '=' });
					if ((int)strArrays1.Length == 2 && strArrays1[0].ToLower() == "initial catalog")
					{
						mapperProviderInfo.Catalog = strArrays1[1];
					}
				}
			}
			mapperProviderInfo.ProviderType = providerType;
			MapperProvider.MapperProviderInfo.Add(name, mapperProviderInfo);
			if (MapperProvider.DefaultProviderName == null)
			{
				MapperProvider.DefaultProviderName = name;
			}
			return mapperProviderInfo;
		}

		internal virtual string ConvertQuery(string text)
		{
			return text;
		}

		public static MapperProvider CreateProvider(string name)
		{
			MapperProvider connectionString = null;
			MapperProvider.MapperProviderList.TryGetValue(name, out connectionString);
			if (connectionString == null)
			{
				Test.Core.MapperProviderInfo mapperProviderInfo = null;
				MapperProvider.MapperProviderInfo.TryGetValue(name, out mapperProviderInfo);
				if (mapperProviderInfo == null)
				{
					throw new Exception(string.Concat(name, " not exist MapperProviderInfo"));
				}
				connectionString = (MapperProvider)Activator.CreateInstance(MapperProvider.MapperProviderType[mapperProviderInfo.ProviderType]);
				connectionString.Name = name;
				connectionString.connectionString = mapperProviderInfo.ConnectionString;
				connectionString.catalog = mapperProviderInfo.Catalog;
				connectionString.Parameter = mapperProviderInfo.Parameter;
				MapperProvider.MapperProviderList.Add(name, connectionString);
			}
			return connectionString;
		}

		public static void Initialize(IEnumerable<Assembly> assemblies, string path)
		{
			CommandRepository.CreateRepository(path);
			MapperProvider.SearchMapperProvider(assemblies.Concat<Assembly>((IEnumerable<Assembly>)(new Assembly[] { typeof(MapperProvider).GetTypeInfo().Assembly })));
			BaseMapper.Initialize(assemblies);
		}

		public abstract int Query(MapperCommand command);

		public abstract int Query(string commandText, MapperParameter parameter = null, CommandType commandType = CommandType.Text);

		public abstract object QueryForScalar(MapperCommand command);

		public abstract object QueryForScalar(string commandText, MapperParameter parameter = null, CommandType commandType = CommandType.Text);

		public ValueRow QueryForValueRow(MapperCommand command)
		{
			return this.QueryForValueSet(command).FirstRow();
		}

		public ValueRow QueryForValueRow(string commandText, MapperParameter parameter = null, CommandType commandType = CommandType.Text)
		{
			return this.QueryForValueSet(commandText, parameter, commandType).FirstRow();
		}

		public abstract ValueSet QueryForValueSet(MapperCommand command);

		public abstract ValueSet QueryForValueSet(string commandText, MapperParameter parameter = null, CommandType commandType = CommandType.Text);

		public ValueTable QueryForValueTable(MapperCommand command)
		{
			return this.QueryForValueSet(command).Tables[0];
		}

		public ValueTable QueryForValueTable(string commandText, MapperParameter parameter = null, CommandType commandType = CommandType.Text)
		{
			return this.QueryForValueSet(commandText, parameter, commandType).Tables[0];
		}

		private static void SearchMapperProvider(IEnumerable<Assembly> assemblies)
		{
			foreach (Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetTypes();
				for (int i = 0; i < (int)types.Length; i++)
				{
					Type type = types[i];
					MapperProviderAttribute customAttribute = type.GetTypeInfo().GetCustomAttribute<MapperProviderAttribute>();
					if (customAttribute != null && !MapperProvider.MapperProviderType.ContainsKey(customAttribute.Name))
					{
						MapperProvider.MapperProviderType.Add(customAttribute.Name, type);
					}
				}
			}
		}
	}
}