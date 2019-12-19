using Test.Core;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace System
{
	public static class StringExtension
	{
		private static Regex rex;

		static StringExtension()
		{
			StringExtension.rex = new Regex("([A-Z])", RegexOptions.Compiled);
		}

		internal static bool MatchDataName(this string source, string target)
		{
			if (target == null)
			{
				return false;
			}
			return source.Replace("_", string.Empty).Equals(target.Replace("_", string.Empty), StringComparison.OrdinalIgnoreCase);
		}

		public static string ToDataName(this string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return name;
			}
			return StringExtension.rex.Replace(name, "_$1").ToUpper().Trim(new char[] { '\u005F' });
		}

		public static ValueTable ToDataTable(this string json)
		{
			throw new NotImplementedException();
		}

		internal static string ToPascalCase(this string name)
		{
			char chr;
			if (string.IsNullOrEmpty(name))
			{
				return name;
			}
			string[] upper = name.Split(new char[] { '\u005F' });
			for (int i = 0; i < (int)upper.Length; i++)
			{
				if (upper[i].Length != 1)
				{
					chr = upper[i][0];
					upper[i] = string.Concat(chr.ToString().ToUpper(), upper[i].Substring(1).ToLower());
				}
				else
				{
					chr = upper[i][0];
					upper[i] = chr.ToString().ToUpper();
				}
			}
			return string.Join(string.Empty, upper);
		}
	}
}