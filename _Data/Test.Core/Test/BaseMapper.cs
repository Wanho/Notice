using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public abstract class BaseMapper : IMapper
	{
		private MapperCombineScope combineContext;

		private static Type typeMapperCommand;

		private static Type typeBaseMapper;

		private static Type typeBaseMapperService;

		private static MethodInfo methodQuery;

		private static MethodInfo methodDataSet;

		private static MethodInfo methodDataTable;

		private static MethodInfo methodDataRow;

		private static MethodInfo methodScalar;

		private static MethodInfo methodToFirstVo;

		private static MethodInfo methodToList;

		private static MethodInfo methodToDictionary;

		protected static Dictionary<string, object> directParameterValue;

		private static FieldInfo fieldDirectParameterValue;

		private static MethodInfo methodDirectParameterValue;

		private static Dictionary<Type, Type> mapperTypes;

		private static object _lock;

		private static ILog logger;

		public MapperParameter DefaultParameter { get; set; } = new MapperParameter();

		public MapperProvider Provider
		{
			get;
			set;
		}

		static BaseMapper()
		{
			BaseMapper.typeMapperCommand = typeof(MapperCommand);
			BaseMapper.typeBaseMapper = typeof(BaseMapper);
			BaseMapper.typeBaseMapperService = typeof(BaseMapperService);
			BaseMapper.methodQuery = typeof(MapperCommand).GetMethod("Query", BindingFlags.Static | BindingFlags.Public);
			BaseMapper.methodDataSet = typeof(MapperCommand).GetMethod("QueryForValueSet", BindingFlags.Static | BindingFlags.Public);
			BaseMapper.methodDataTable = typeof(MapperCommand).GetMethod("QueryForValueTable", BindingFlags.Static | BindingFlags.Public);
			BaseMapper.methodDataRow = typeof(MapperCommand).GetMethod("QueryForValueRow", BindingFlags.Static | BindingFlags.Public);
			BaseMapper.methodScalar = typeof(MapperCommand).GetMethod("QueryForScalar", BindingFlags.Static | BindingFlags.Public);
			BaseMapper.methodToFirstVo = typeof(ValueTableExtension).GetMethod("ToFirstVo", new Type[] { typeof(ValueTable) });
			BaseMapper.methodToList = typeof(ValueTableExtension).GetMethod("ToList", BindingFlags.Static | BindingFlags.Public);
			BaseMapper.methodToDictionary = typeof(ValueTableExtension).GetMethod("ToDictionary", BindingFlags.Static | BindingFlags.Public);
			BaseMapper.directParameterValue = new Dictionary<string, object>();
			BaseMapper.fieldDirectParameterValue = typeof(BaseMapper).GetField("directParameterValue", BindingFlags.Static | BindingFlags.NonPublic);
			BaseMapper.methodDirectParameterValue = BaseMapper.directParameterValue.GetType().GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);
			BaseMapper.mapperTypes = new Dictionary<Type, Type>();
			BaseMapper._lock = new object();
			BaseMapper.logger = LogManager.GetLogger("Test.BaseMapper");
		}

		public BaseMapper()
		{
			this.Provider = MapperProvider.DefaultProvider;
		}

		public BaseMapper(MapperProvider provider)
		{
			this.Provider = provider;
		}

		public MapperCombineScope BeginCombineScope()
		{
			if (this.combineContext == null)
			{
				this.combineContext = new MapperCombineScope(this);
			}
			this.combineContext.refCount++;
			return this.combineContext;
		}

		public MapperCombineScope BeginCombineScope(bool combine)
		{
			MapperCombineScope mapperCombineScope = this.BeginCombineScope();
			mapperCombineScope.noCombine = !combine;
			return mapperCombineScope;
		}

		internal static Type BuildMapperAssembly(Type mapperInterfaceType)
		{
			object name;
			object obj;
			BaseMapper.logger.Debug(string.Concat("BuildMapperAssembly ", mapperInterfaceType.FullName), (LogInfo)null, null);
			string str = string.Concat(mapperInterfaceType.Namespace, ".Concrete");
			AppDomain currentDomain = AppDomain.CurrentDomain;
			AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(str), AssemblyBuilderAccess.Run);
			string str1 = string.Concat(str, ".", mapperInterfaceType.Name.Substring(1));
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(str1);
			TypeBuilder typeBuilder = moduleBuilder.DefineType(str1, TypeAttributes.Public, BaseMapper.typeBaseMapper);
			NameAttribute customAttribute = mapperInterfaceType.GetTypeInfo().GetCustomAttribute<NameAttribute>();
			if (customAttribute != null)
			{
				name = customAttribute.Name;
			}
			else
			{
				name = null;
			}
			if (name == null)
			{
				name = mapperInterfaceType.Name.Substring(1, mapperInterfaceType.Name.Length - "IMapper".Length);
			}
			string str2 = (string)name;
			typeBuilder.AddInterfaceImplementation(mapperInterfaceType);
			AnonymousTypeBuilder anonymousTypeBuilder = new AnonymousTypeBuilder(assemblyBuilder, moduleBuilder);
			MethodInfo[] methods = mapperInterfaceType.GetMethods();
			for (int i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				ParameterInfo[] parameters = methodInfo.GetParameters();
				Type[] array = (
					from p in (IEnumerable<ParameterInfo>)methodInfo.GetParameters()
					select p.ParameterType).ToArray<Type>();
				List<ParameterAttribute> list = methodInfo.GetCustomAttributes<ParameterAttribute>().ToList<ParameterAttribute>();
				MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.NewSlot, CallingConventions.Standard, methodInfo.ReturnType, array);
				for (byte j = 0; j < (int)array.Length; j = (byte)(j + 1))
				{
					methodBuilder.DefineParameter(j + 1, ParameterAttributes.None, parameters[j].Name);
				}
				ILGenerator lGenerator = methodBuilder.GetILGenerator();
				if (methodInfo.ReturnType != typeof(void))
				{
					lGenerator.DeclareLocal(methodInfo.ReturnType);
				}
				lGenerator.Emit(OpCodes.Nop);
				lGenerator.Emit(OpCodes.Ldarg_0);
				lGenerator.Emit(OpCodes.Call, BaseMapper.typeBaseMapper.GetMethod("get_Provider", BindingFlags.Instance | BindingFlags.Public));
				NameAttribute nameAttribute = methodInfo.GetCustomAttribute<NameAttribute>();
				if (nameAttribute != null)
				{
					obj = nameAttribute.Name;
				}
				else
				{
					obj = null;
				}
				if (obj == null)
				{
					obj = string.Concat(str2, ".", methodInfo.Name);
				}
				string str3 = (string)obj;
				lGenerator.Emit(OpCodes.Ldstr, str3);
				if (array.Length != 0 || list.Count != 0)
				{
					sbyte num = 0;
					bool[] flagArray = new bool[(int)array.Length + list.Count];
					for (byte k = 0; k < (int)array.Length; k = (byte)(k + 1))
					{
						flagArray[k] = array[k].GetTypeInfo().IsSubclassOf(typeof(ValueObject));
						if (flagArray[k])
						{
							num = (sbyte)(num + 1);
						}
					}
					for (byte l = 0; l < list.Count; l = (byte)(l + 1))
					{
						flagArray[(int)array.Length + l] = list[l].Parameter.Value.GetType().GetTypeInfo().IsSubclassOf(typeof(ValueObject));
						if (flagArray[(int)array.Length + l])
						{
							num = (sbyte)(num + 1);
						}
					}
					Type[] type = new Type[(int)array.Length + list.Count - num];
					string[] parameterName = new string[(int)array.Length + list.Count - num];
					BaseMapper.EmitHepler(lGenerator, OpCodes.Ldc_I4_S, num + (type.Length == 0 ? 0 : 1));
					lGenerator.Emit(OpCodes.Newarr, typeof(object));
					lGenerator.Emit(OpCodes.Dup);
					sbyte num1 = 0;
					for (byte m = 0; m < (int)array.Length; m = (byte)(m + 1))
					{
						if (!flagArray[m])
						{
							type[m - num1] = array[m];
							parameterName[m - num1] = parameters[m].Name;
						}
						else
						{
							OpCode ldcI4S = OpCodes.Ldc_I4_S;
							sbyte num2 = num1;
							num1 = (sbyte)(num2 + 1);
							BaseMapper.EmitHepler(lGenerator, ldcI4S, num2);
							BaseMapper.EmitHepler(lGenerator, OpCodes.Ldarg_S, m + 1);
							lGenerator.Emit(OpCodes.Stelem_Ref);
							if (m == (int)array.Length - (type.Length == 0 ? 2 : 1))
							{
								lGenerator.Emit(OpCodes.Dup);
							}
						}
					}
					for (byte n = 0; n < list.Count; n = (byte)(n + 1))
					{
						if (!flagArray[(int)array.Length + n])
						{
							BaseMapper.directParameterValue.Add(string.Concat(str1, ".", methodInfo.Name), list[n].Parameter.Value);
							type[(int)array.Length + n - num1] = list[n].Parameter.Value.GetType();
							parameterName[(int)array.Length + n - num1] = list[n].Parameter.ParameterName;
						}
					}
					if (type.Length != 0)
					{
						BaseMapper.EmitHepler(lGenerator, OpCodes.Ldc_I4_S, num);
						for (byte o = 0; o < (int)array.Length; o = (byte)(o + 1))
						{
							if (!flagArray[o])
							{
								BaseMapper.EmitHepler(lGenerator, OpCodes.Ldarg_S, o + 1);
							}
						}
						for (byte p1 = 0; p1 < list.Count; p1 = (byte)(p1 + 1))
						{
							if (!flagArray[(int)array.Length + p1])
							{
								lGenerator.Emit(OpCodes.Ldsfld, BaseMapper.fieldDirectParameterValue);
								lGenerator.Emit(OpCodes.Ldstr, string.Concat(str1, ".", methodInfo.Name));
								lGenerator.Emit(OpCodes.Callvirt, BaseMapper.methodDirectParameterValue);
							}
						}
						Type type1 = anonymousTypeBuilder.CreateType(type, parameterName);
						lGenerator.Emit(OpCodes.Newobj, type1.GetConstructors()[0]);
						lGenerator.Emit(OpCodes.Stelem_Ref);
					}
				}
				else
				{
					MethodInfo method = typeof(Array).GetMethod("Empty");
					method = method.MakeGenericMethod(new Type[] { typeof(object) });
					lGenerator.Emit(OpCodes.Call, method);
				}
				CommandItem item = CommandRepository.Instance[str3] ?? new CommandItem()
				{
					Verb = CommandVerb.Get
				};
				MethodInfo methodInfo1 = (methodInfo.ReturnType == typeof(void) || item.Verb != CommandVerb.Get ? BaseMapper.methodQuery : BaseMapper.methodScalar);
				TypeInfo typeInfo = methodInfo.ReturnType.GetTypeInfo();
				bool flag = typeInfo.ImplementedInterfaces.Contains<Type>(typeof(IList));
				bool flag1 = typeInfo.ImplementedInterfaces.Contains<Type>(typeof(IDictionary));
				bool flag2 = typeInfo.IsSubclassOf(typeof(ValueObject));
				bool flag3 = false;
				if (methodInfo.ReturnType == typeof(ValueSet))
				{
					methodInfo1 = BaseMapper.methodDataSet;
				}
				else if (methodInfo.ReturnType == typeof(ValueTable))
				{
					methodInfo1 = BaseMapper.methodDataTable;
				}
				else if (methodInfo.ReturnType == typeof(ValueRow))
				{
					methodInfo1 = BaseMapper.methodDataRow;
				}
				else if (item.Verb == CommandVerb.Get && flag | flag1 | flag2)
				{
					methodInfo1 = BaseMapper.methodDataTable;
					flag3 = true;
				}
				if (methodInfo1 == BaseMapper.methodScalar)
				{
					methodInfo1 = methodInfo1.MakeGenericMethod(new Type[] { methodInfo.ReturnType });
				}
				lGenerator.Emit(OpCodes.Call, methodInfo1);
				if (flag3)
				{
					MethodInfo methodInfo2 = null;
					if (!flag)
					{
						methodInfo2 = (!flag1 ? BaseMapper.methodToFirstVo.MakeGenericMethod(new Type[] { methodInfo.ReturnType }) : BaseMapper.methodToDictionary.MakeGenericMethod(new Type[] { methodInfo.ReturnType.GenericTypeArguments[0], methodInfo.ReturnType.GenericTypeArguments[1] }));
					}
					else
					{
						methodInfo2 = BaseMapper.methodToList.MakeGenericMethod(new Type[] { methodInfo.ReturnType.GenericTypeArguments[0] });
					}
					lGenerator.Emit(OpCodes.Call, methodInfo2);
				}
				if (methodInfo.ReturnType != typeof(void))
				{
					lGenerator.Emit(OpCodes.Stloc_0);
					lGenerator.Emit(OpCodes.Br_S, (sbyte)0);
					lGenerator.Emit(OpCodes.Ldloc_0);
				}
				else
				{
					lGenerator.Emit(OpCodes.Pop);
				}
				lGenerator.Emit(OpCodes.Ret);
			}
			return typeBuilder.CreateTypeInfo().AsType();
		}

		public static T CreateMapper<T>(MapperProvider provider = null)
		where T : IMapper
		{
			Type type = typeof(T);
			Type item = null;
			if (!BaseMapper.mapperTypes.TryGetValue(type, out item))
			{
				BaseMapper.LoadAssembliy(type.GetTypeInfo().Assembly);
				if (!BaseMapper.mapperTypes.ContainsKey(type))
				{
					throw new Exception(string.Concat(type.FullName, " not exist mapper concrete"));
				}
				item = BaseMapper.mapperTypes[type];
			}
			T t = (T)Activator.CreateInstance(item);
			t.Provider = provider ?? MapperProvider.DefaultProvider;
			return t;
		}

		public static T CreateMapper<T>(string provider)
		where T : IMapper
		{
			return BaseMapper.CreateMapper<T>(MapperProvider.CreateProvider(provider));
		}

		internal static IMapper CreateMapper(Type type, MapperProvider provider = null)
		{
			Type item = null;
			if (!BaseMapper.mapperTypes.TryGetValue(type, out item))
			{
				BaseMapper.LoadAssembliy(type.GetTypeInfo().Assembly);
				if (!BaseMapper.mapperTypes.ContainsKey(type))
				{
					throw new Exception(string.Concat(type.FullName, " not exist mapper concrete"));
				}
				item = BaseMapper.mapperTypes[type];
			}
			IMapper mapper = (IMapper)Activator.CreateInstance(item);
			mapper.Provider = provider ?? MapperProvider.DefaultProvider;
			return mapper;
		}

		private static void EmitHepler(ILGenerator gen, OpCode code, int idx)
		{
			if (code != OpCodes.Ldc_I4_S)
			{
				if (code != OpCodes.Ldarg_S)
				{
					throw new Exception();
				}
				switch (idx)
				{
					case 0:
					{
						gen.Emit(OpCodes.Ldarg_0);
						return;
					}
					case 1:
					{
						gen.Emit(OpCodes.Ldarg_1);
						return;
					}
					case 2:
					{
						gen.Emit(OpCodes.Ldarg_2);
						return;
					}
					case 3:
					{
						gen.Emit(OpCodes.Ldarg_3);
						return;
					}
				}
				gen.Emit(OpCodes.Ldarg_S, (sbyte)idx);
				return;
			}
			switch (idx)
			{
				case -1:
				{
					gen.Emit(OpCodes.Ldc_I4_M1);
					return;
				}
				case 0:
				{
					gen.Emit(OpCodes.Ldc_I4_0);
					return;
				}
				case 1:
				{
					gen.Emit(OpCodes.Ldc_I4_1);
					return;
				}
				case 2:
				{
					gen.Emit(OpCodes.Ldc_I4_2);
					return;
				}
				case 3:
				{
					gen.Emit(OpCodes.Ldc_I4_3);
					return;
				}
				case 4:
				{
					gen.Emit(OpCodes.Ldc_I4_4);
					return;
				}
				case 5:
				{
					gen.Emit(OpCodes.Ldc_I4_5);
					return;
				}
				case 6:
				{
					gen.Emit(OpCodes.Ldc_I4_6);
					return;
				}
				case 7:
				{
					gen.Emit(OpCodes.Ldc_I4_7);
					return;
				}
				case 8:
				{
					gen.Emit(OpCodes.Ldc_I4_8);
					return;
				}
			}
			gen.Emit(OpCodes.Ldc_I4_S, (sbyte)idx);
		}

		internal void EndCombineScope()
		{
			this.combineContext = null;
		}

		public MapperCommand GetCommand(string commandId, params object[] source)
		{
			return MapperCommand.GetCommand(this.Provider, commandId, source);
		}

		public static void Initialize(IEnumerable<Assembly> assemblies)
		{
			BaseMapper.logger.Debug("Initialize start", (LogInfo)null, null);
			foreach (Assembly assembly in assemblies)
			{
				BaseMapper.LoadAssembliy(assembly);
			}
		}

		public static void LoadAssembliy(Assembly assembly)
		{
			BaseMapper.logger.Debug(string.Concat("LoadAssembliy ", assembly.FullName), (LogInfo)null, null);
			foreach (Type type1 in (
				from type in (IEnumerable<Type>)assembly.GetTypes()
				select new { type = type, typeInfo = type.GetTypeInfo() }).Where((argument0) => {
				if (!argument0.typeInfo.ImplementedInterfaces.Contains<Type>(typeof(IMapper)))
				{
					return false;
				}
				return !argument0.typeInfo.IsSubclassOf(typeof(BaseMapper));
			}).Select((argument1) => argument1.type))
			{
				if (BaseMapper.mapperTypes.ContainsKey(type1))
				{
					continue;
				}
				Type type2 = BaseMapper.BuildMapperAssembly(type1);
				BaseMapper.mapperTypes.Add(type1, type2);
			}
		}

		public static void LoadMapper<T>(ref T iMapper, MapperProvider provider = null)
		where T : IMapper
		{
			Type type = typeof(T);
			Type item = null;
			if (!BaseMapper.mapperTypes.TryGetValue(type, out item))
			{
				BaseMapper.LoadAssembliy(type.GetTypeInfo().Assembly);
				if (!BaseMapper.mapperTypes.ContainsKey(type))
				{
					throw new Exception(string.Concat(type.FullName, " not exist mapper concrete"));
				}
				item = BaseMapper.mapperTypes[type];
			}
			iMapper = (T)Activator.CreateInstance(item);
			iMapper.Provider = provider ?? MapperProvider.DefaultProvider;
		}
	}
}