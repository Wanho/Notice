using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public abstract class BaseMapperService
	{
		private static object _lock;

		private static Dictionary<Type, List<BaseMapperService.MapperMemberInfo>> mapperMemberInfo;

		private ModelLoader loader;

		static BaseMapperService()
		{
			BaseMapperService._lock = new object();
			BaseMapperService.mapperMemberInfo = new Dictionary<Type, List<BaseMapperService.MapperMemberInfo>>();
		}

		public BaseMapperService()
		{
			BaseMapperService.CreateMapperMember(this);
		}

		public BaseMapperService(bool autoCreateMapper)
		{
			if (autoCreateMapper)
			{
				BaseMapperService.CreateMapperMember(this);
			}
		}

		public void ChangeInterval(int interval, bool load = false)
		{
			if (this.loader == null)
			{
				throw new Exception(string.Concat(this.loader.type.Name, "not exist model loader"));
			}
			this.loader.Start(interval, load);
		}

		public static void CreateMapperMember(object model)
		{
			Type type = model.GetType();
			List<BaseMapperService.MapperMemberInfo> mapperMemberInfos = null;
			if (!BaseMapperService.mapperMemberInfo.TryGetValue(type, out mapperMemberInfos))
			{
				lock (BaseMapperService._lock)
				{
					if (!BaseMapperService.mapperMemberInfo.TryGetValue(type, out mapperMemberInfos))
					{
						mapperMemberInfos = new List<BaseMapperService.MapperMemberInfo>();
						FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						for (int i = 0; i < (int)fields.Length; i++)
						{
							FieldInfo fieldInfo = fields[i];
							Type fieldType = fieldInfo.FieldType;
							TypeInfo typeInfo = fieldType.GetTypeInfo();
							if (typeInfo.ImplementedInterfaces.Contains<Type>(typeof(IMapper)) && !typeInfo.IsSubclassOf(typeof(BaseMapper)) && fieldInfo.GetValue(model) == null)
							{
								List<ProviderParameterAttribute> list = fieldType.GetCustomAttributes<ProviderParameterAttribute>().ToList<ProviderParameterAttribute>();
								MapperProviderAttribute customAttribute = fieldInfo.GetCustomAttribute<MapperProviderAttribute>();
								List<ProviderParameterAttribute> providerParameterAttributes = fieldInfo.GetCustomAttributes<ProviderParameterAttribute>().ToList<ProviderParameterAttribute>();
								foreach (ProviderParameterAttribute providerParameterAttribute in list)
								{
									if (providerParameterAttributes.Exists((ProviderParameterAttribute p) => p.Name == providerParameterAttribute.Name))
									{
										continue;
									}
									providerParameterAttributes.Add(providerParameterAttribute);
								}
								if (customAttribute != null)
								{
									MapperProvider mapperProvider = MapperProvider.CreateProvider(customAttribute.Name);
									mapperMemberInfos.Add(new BaseMapperService.MapperMemberInfo()
									{
										Field = fieldInfo,
										Type = fieldType,
										Parameter = list,
										Provider = mapperProvider
									});
								}
								else
								{
									mapperMemberInfos.Add(new BaseMapperService.MapperMemberInfo()
									{
										Field = fieldInfo,
										Type = fieldType,
										Parameter = list
									});
								}
							}
						}
						BaseMapperService.mapperMemberInfo.Add(type, mapperMemberInfos);
					}
				}
			}
			foreach (BaseMapperService.MapperMemberInfo mapperMemberInfo in mapperMemberInfos)
			{
				IMapper mapper = BaseMapper.CreateMapper(mapperMemberInfo.Type, mapperMemberInfo.Provider);
				mapperMemberInfo.Field.SetValue(model, mapper);
			}
		}

		public void LoadImmediately()
		{
			if (this.loader == null)
			{
				throw new Exception(string.Concat(this.loader.type.Name, " not exist model loader"));
			}
			this.loader.Start(this.loader.interval, true);
		}

		internal void SetModelLoader(ModelLoader loader)
		{
			this.loader = loader;
		}

		private class MapperMemberInfo
		{
			public FieldInfo Field;

			public Type Type;

			public MapperProvider Provider;

			public List<ProviderParameterAttribute> Parameter;

			public MapperMemberInfo()
			{
			}
		}
	}
}