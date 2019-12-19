using System.Collections.Generic;

namespace System
{
	public class FileTypeCode
	{
		internal static Dictionary<string, WebPath> fileRoots;

		static FileTypeCode()
		{
			FileTypeCode.fileRoots = new Dictionary<string, WebPath>();
		}

		public FileTypeCode()
		{
		}

		public static void AddRoot(string name, string absolutePath, string path)
		{
			FileTypeCode.fileRoots.Add(name, new WebPath()
			{
				AbsolutePath = absolutePath,
				Path = path
			});
		}

		public static WebPath FromAbsolutePath(string absolutePath)
		{
			WebPath webPath;
			Dictionary<string, WebPath>.ValueCollection.Enumerator enumerator = FileTypeCode.fileRoots.Values.GetEnumerator();
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
}