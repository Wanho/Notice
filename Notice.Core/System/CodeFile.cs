using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace System
{
    public class CodeFileType
    {
        internal static Dictionary<string, WebPath> fileRoots;

        static CodeFileType()
        {
            CodeFileType.fileRoots = new Dictionary<string, WebPath>();
        }

        public CodeFileType()
        {
        }

        public static void AddRoot(string name, string absolutePath, string path)
        {
            CodeFileType.fileRoots.Add(name, new WebPath()
            {
                AbsolutePath = absolutePath,
                Path = path
            });
        }

        public static WebPath FromAbsolutePath(string absolutePath)
        {
            WebPath webPath;
            Dictionary<string, WebPath>.ValueCollection.Enumerator enumerator = CodeFileType.fileRoots.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    WebPath current = enumerator.Current;
                    if (!absolutePath.StartsWith(current.AbsolutePath, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    absolutePath = absolutePath.Substring(current.AbsolutePath.Length - 1);
                    WebPath webPath1 = new WebPath()
                    {
                        AbsolutePath = string.Concat(current.AbsolutePath.TrimEnd(new char[] { '/' }), absolutePath),
                        Path = string.Concat(current.Path.TrimEnd(new char[] { '\\' }), absolutePath.Replace("/", "\\"))
                    };
                    webPath = webPath1;
                    return webPath;
                }
                throw new Exception("can not found root in fileroots");
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }
            return webPath;
        }
    }

    [Custom_NotInitType]
    public class CodeFileType<T> : BaseCode<T>, ICode where T : CodeFileType<T>, new()
    {
        public WebPath RootPath { get; internal set; }
        public string Value { get; set; }

        protected CodeFileType()
        {
        }

        public static CodeFileType<T> FindCode(object value)
        {
            string str = value as string;
            return BaseCode<T>.Codes.Find((T p) => p.Value.Equals(str, StringComparison.OrdinalIgnoreCase));
        }

        public static CodeFileType<T> FindCode_(string str)
        {
            return BaseCode<T>.Codes.Find((T p) => p.Value.Equals(str, StringComparison.OrdinalIgnoreCase));
        }

        public WebPath GetDirectoryPath(string sub = null, DateTime? dt = null)
        {
            if (!string.IsNullOrEmpty(sub))
            {
                sub = sub.Replace("\\", "/");
                if (!sub.EndsWith("/"))
                {
                    sub = string.Concat(sub, "/");
                }
            }
            return this.GetPath(sub, dt);
        }

        public WebPath GetDirectoryPath(DateTime dt)
        {
            return this.GetPath(null, new DateTime?(dt));
        }

        public WebPath GetFilePath(string fileName, DateTime? dt = null)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = fileName.Replace("\\", "/");
            }
            return this.GetPath(fileName, dt);
        }

        private WebPath GetPath(string fileName, DateTime? dt)
        {
            string empty = string.Empty;
            string value = this.Value;
            Custom_CodeFileTypeAttribute customAttribute = base.GetType().GetField(base.Name).GetCustomAttribute<Custom_CodeFileTypeAttribute>();
            if (customAttribute != null)
            {
                if (!string.IsNullOrEmpty(customAttribute.DateFormat))
                {
                    if (!dt.HasValue)
                    {
                        throw new Exception(string.Concat(base.Name, " needs DateTime"));
                    }
                    DateTime dateTime = dt.Value;
                    empty = string.Concat(dateTime.ToString(customAttribute.DateFormat), "/");
                }
                if (!string.IsNullOrEmpty(customAttribute.DirectoryName))
                {
                    value = customAttribute.DirectoryName;
                }
            }
            if (fileName != null && fileName.StartsWith("/")) {
                fileName = fileName.Substring(1);
            }
            WebPath webPath = this.ToWebPath(string.Format("{0}{1}/{2}{3}", new object[] { this.RootPath.AbsolutePath, value, empty, fileName }));
            string directoryName = Path.GetDirectoryName(webPath.Path);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            return webPath;
        }

        public static explicit operator CodeFileType<T>(string value)
        {
            return BaseCode<T>.Codes.Find((T p) => p.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public static explicit operator String(CodeFileType<T> code)
        {
            return code.Value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public WebPath ToWebPath(string absolutePath)
        {
            if (!absolutePath.StartsWith(this.RootPath.AbsolutePath, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(string.Concat("path is must be start with '", this.RootPath.AbsolutePath, "'"));
            }
            absolutePath = absolutePath.Substring(this.RootPath.AbsolutePath.Length - 1);
            WebPath webPath = new WebPath()
            {
                AbsolutePath = string.Concat(this.RootPath.AbsolutePath.TrimEnd(new char[] { '/' }), absolutePath),
                Path = string.Concat(this.RootPath.Path.TrimEnd(new char[] { '\\' }), absolutePath.Replace("/", "\\"))
            };
            return webPath;
        }
    }
}
