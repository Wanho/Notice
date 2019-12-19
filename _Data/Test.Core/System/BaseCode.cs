using Test.Core;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Test.Core;

namespace System
{
	public abstract class BaseCode
	{
		internal static long startSequence;

		static BaseCode()
		{
		}

		protected BaseCode()
		{
		}
	}

    [NotInitType]
    public abstract class BaseCode<TCode> : BaseCode
    where TCode : BaseCode<TCode>
    {
        public readonly static TCode Nothing;

        internal static TCode Default;

        protected CodeAttribute attribute;

        private static ILog logger;

        protected readonly static BaseCode<TCode>.CodeSetting<TCode> setting;

        public static Dictionary<string, List<TCode>> CodeGroups
        {
            get;
            private set;
        }

        public static List<TCode> Codes
        {
            get;
            private set;
        }

        public Dictionary<string, string> Data
        {
            get;
            internal set;
        }

        public string[] Groups
        {
            get;
            internal set;
        }

        public string Name
        {
            get;
            internal set;
        }

        public int Order
        {
            get;
            internal set;
        }

        static BaseCode()
        {
            BaseCode<TCode>.Default = default(TCode);
            BaseCode<TCode>.CodeGroups = new Dictionary<string, List<TCode>>();
            BaseCode<TCode>.logger = LogManager.GetLogger("Test.Code");
            BaseCode<TCode>.setting = new BaseCode<TCode>.CodeSetting<TCode>();
        }

        protected BaseCode()
        {
        }

        private static string _ToSequence(long num)
        {
            char chr;
            string empty = string.Empty;
            while (true)
            {
                long num1 = num / (long)36;
                long num2 = num % (long)36;
                if (num2 >= (long)10)
                {
                    chr = (char)((long)65 + (num2 - (long)10));
                    empty = string.Concat(chr.ToString(), empty);
                }
                else
                {
                    chr = (char)((long)48 + num2);
                    empty = string.Concat(chr.ToString(), empty);
                }
                if (num1 == 0)
                {
                    break;
                }
                num = num1;
            }
            return empty;
        }

        public static TCode Find(string name)
        {
            if (name == null)
            {
                return default(TCode);
            }
            return BaseCode<TCode>.Codes.Find((TCode p) => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        protected static void Init()
        {
            object value;
            BaseCode<TCode>.setting.Tick = BaseCode.startSequence;
            Type type = typeof(TCode);
            BaseCode<TCode>.logger.Debug(string.Concat("Load ", type.FullName), (LogInfo)null, null);
            List<TCode> tCodes = new List<TCode>();
            int order = 0;
            foreach (FieldInfo runtimeField in type.GetRuntimeFields())
            {
                if (runtimeField.FieldType != type || runtimeField.Name == "Nothing")
                {
                    continue;
                }
                TCode name = Activator.CreateInstance<TCode>();
                name.Name = runtimeField.Name;
                runtimeField.SetValue(null, name);
                tCodes.Add(name);
                CodeAttribute customAttribute = runtimeField.GetCustomAttribute<CodeAttribute>();
                CodeGroupAttribute codeGroupAttribute = runtimeField.GetCustomAttribute<CodeGroupAttribute>();
                List<CodeDataAttribute> list = runtimeField.GetCustomAttributes<CodeDataAttribute>().ToList<CodeDataAttribute>();
                name.attribute = customAttribute;
                if (customAttribute != null)
                {
                    if (customAttribute.Order >= 0)
                    {
                        order = customAttribute.Order;
                    }
                    name.Groups = customAttribute.Group;
                }
                if (codeGroupAttribute != null)
                {
                    name.Groups = codeGroupAttribute.Group;
                }
                if (name.Groups != null)
                {
                    string[] groups = name.Groups;
                    for (int i = 0; i < (int)groups.Length; i++)
                    {
                        string str = groups[i];
                        if (!BaseCode<TCode>.CodeGroups.ContainsKey(str))
                        {
                            BaseCode<TCode>.CodeGroups.Add(str, new List<TCode>());
                        }
                        BaseCode<TCode>.CodeGroups[str].Add(name);
                    }
                }
                if (list.Count > 0)
                {
                    name.Data = new Dictionary<string, string>();
                    foreach (CodeDataAttribute codeDataAttribute in list)
                    {
                        name.Data.Add(codeDataAttribute.Name, codeDataAttribute.Value);
                    }
                }
                int num = order;
                order = num + 1;
                name.Order = num;
                if ((object)name is ICode)
                {
                    ICode code = (object)name as ICode;
                    if (customAttribute != null)
                    {
                        value = customAttribute.Value;
                    }
                    else
                    {
                        value = null;
                    }
                    if (value == null)
                    {
                        value = name.Name;
                    }
                    code.Value = (string)value;
                }
                else if ((object)name is INumericCode)
                {
                    ((object)name as INumericCode).Value = (customAttribute != null ? customAttribute.NumericValue : name.Order);
                }
                if (type.GetTypeInfo().BaseType.FullName.IndexOf("FileTypeCode") >= 0)
                {
                    FileRootAttribute fileRootAttribute = runtimeField.GetCustomAttribute<FileRootAttribute>();
                    if (fileRootAttribute == null)
                    {
                        fileRootAttribute = runtimeField.DeclaringType.GetTypeInfo().GetCustomAttribute<FileRootAttribute>();
                        if (fileRootAttribute != null)
                        {
                            PropertyInfo property = type.GetProperty("RootPath");
                            if (!FileTypeCode.fileRoots.ContainsKey(fileRootAttribute.Name))
                            {
                                throw new Exception(string.Concat(fileRootAttribute.Name, ": not exist root config"));
                            }
                            property.SetValue(name, FileTypeCode.fileRoots[fileRootAttribute.Name]);
                        }
                        else
                        {
                            BaseCode<TCode>.logger.Warn(string.Concat(runtimeField.Name, "not exist root attribute"), (LogInfo)null, null);
                        }
                    }
                }
                if (BaseCode<TCode>.Default == null || runtimeField.GetCustomAttribute<DefaultCodeAttribute>() == null)
                {
                    continue;
                }
                BaseCode<TCode>.Default = name;
            }

            BaseCode<TCode>.Codes = (from p in tCodes orderby p.Order, p.Name select p).ToList<TCode>();
        }

        public static Sequence NextSequence()
        {
            return 
                new Sequence(
                    string.Concat( 
                        BaseCode<TCode>._ToSequence(
                            Interlocked.Increment(ref BaseCode<TCode>.setting.Tick)
                       ).PadLeft( 
                            BaseCode<TCode>.setting.CodeLength - BaseCode<TCode>.setting.Suffix.Length, '0'), BaseCode<TCode>.setting.Suffix
                    )
                );
        }

        public static long ParseSequence(string seq)
        {
            seq = seq.Substring(0, seq.Length - BaseCode<TCode>.setting.Suffix.Length);
            long num = (long)0;
            string str = seq;
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                num = (chr >= 'A' ? num * (long)36 + (long)(chr - 65 + 10) : num * (long)36 + (long)(chr - 48));
            }
            return num;
        }

        public static Sequence ToSequence(long num)
        {
            return new Sequence(string.Concat(BaseCode<TCode>._ToSequence(num).PadLeft(BaseCode<TCode>.setting.CodeLength - BaseCode<TCode>.setting.Suffix.Length, '0'), BaseCode<TCode>.setting.Suffix));
        }

        protected sealed class CodeSetting<K>
        {
            public byte CodeLength;

            public string Suffix;

            public long Tick;

            public CodeSetting()
            {
            }
        }
    }

    public abstract class BaseCodeInitializer
    {
        private static ILog logger;

        static BaseCodeInitializer()
        {
            BaseCodeInitializer.logger = LogManager.GetLogger("Test.Code");
        }

        protected BaseCodeInitializer()
        {
        }

        public static void Initialize(IEnumerable<Assembly> assemblies, long startSequence = -1L)
        {
            BaseCodeInitializer.logger.Debug("Code Initialize start", (LogInfo)null, null);
            assemblies = assemblies.Concat<Assembly>((IEnumerable<Assembly>)(new Assembly[] { typeof(BaseCode).GetTypeInfo().Assembly }));
            if (BaseCode.startSequence >= (long)0)
            {
                BaseCode.startSequence = startSequence;
            }
            foreach (Assembly assembly in assemblies)
            {
                BaseCodeInitializer.logger.Debug(string.Concat("load code ", assembly.FullName), (LogInfo)null, null);
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < (int)types.Length; i++)
                {
                    Type type = types[i];
                    string name = type.Name;
                    if (type.GetTypeInfo().IsSubclassOf(typeof(BaseCode)) && type.GetTypeInfo().GetCustomAttribute<NotInitTypeAttribute>(false) == null)
                    {
                        type.GetMethod("Init", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Invoke(null, null);
                    }
                }
            }
        }
    }
}