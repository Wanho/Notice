using Notice.Core;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace System
{
    public abstract class BaseCode
    {
        internal static long start;

        static BaseCode()
        {
        }

        protected BaseCode()
        {
        }
    }

    [NotInitType]
    public abstract class BaseCode<T> : BaseCode where T : BaseCode<T>
    {
        public readonly static T Nothing;
        internal static T Default;
        protected CodeAttribute attribute;

        protected readonly static BaseCode<T>.CodeSetting<T> setting;

        public static Dictionary<string, List<T>> CodeGroups { get; private set; }
        public static List<T> Codes { get; private set; }
        public Dictionary<string, string> Data { get; internal set; }
        public string[] Groups { get; internal set; }
        public string Name { get; internal set; }
        public int Order { get; internal set; }

        static BaseCode()
        {
            BaseCode<T>.Default = default(T);
            BaseCode<T>.CodeGroups = new Dictionary<string, List<T>>();
            BaseCode<T>.setting = new BaseCode<T>.CodeSetting<T>();
        }

        protected BaseCode()
        {
        }

        public static T Find(string name)
        {
            if (name == null)
            {
                return default(T);
            }
            return BaseCode<T>.Codes.Find((T p) => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        protected static void Init()
        {
            object value;
            BaseCode<T>.setting.Tick = BaseCode.start;
            Type type = typeof(T);
            List<T> tCodes = new List<T>();
            int order = 0;
            foreach (FieldInfo runtimeField in type.GetRuntimeFields())
            {
                if (runtimeField.FieldType != type || runtimeField.Name == "Nothing") {
                    continue;
                }

                T name = Activator.CreateInstance<T>();
                name.Name = runtimeField.Name;
                runtimeField.SetValue(null, name);
                tCodes.Add(name);
                CodeAttribute attr = runtimeField.GetCustomAttribute<CodeAttribute>();
                CodeGroupAttribute codeGroupAttribute = runtimeField.GetCustomAttribute<CodeGroupAttribute>();
                List<CodeDataAttribute> list = runtimeField.GetCustomAttributes<CodeDataAttribute>().ToList<CodeDataAttribute>();
                name.attribute = attr;
                if (attr != null) {
                    if (attr.Order >= 0) {
                        order = attr.Order;
                    }
                    name.Groups = attr.Group;
                }
                if (codeGroupAttribute != null) {
                    name.Groups = codeGroupAttribute.Group;
                }
                if (name.Groups != null) {
                    string[] groups = name.Groups;
                    for (int i = 0; i < (int)groups.Length; i++) {
                        string str = groups[i];
                        if (!BaseCode<T>.CodeGroups.ContainsKey(str))
                            BaseCode<T>.CodeGroups.Add(str, new List<T>());

                        BaseCode<T>.CodeGroups[str].Add(name);
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
                    if (attr != null)
                    {
                        value = attr.Value;
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
                    ((object)name as INumericCode).Value = (attr != null ? attr.NumericValue : name.Order);
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
                            // BaseCode<T>.logger.Warn(string.Concat(runtimeField.Name, "not exist root attribute"), (LogInfo)null, null);
                        }
                    }
                }
                if (BaseCode<T>.Default == null || runtimeField.GetCustomAttribute<DefaultCodeAttribute>() == null)
                {
                    continue;
                }
                BaseCode<T>.Default = name;
            }

            BaseCode<T>.Codes = (from p in tCodes orderby p.Order, p.Name select p).ToList<T>();
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
}
