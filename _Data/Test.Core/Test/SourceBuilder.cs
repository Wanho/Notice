using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Test.Core
{
	public sealed class SourceBuilder
	{
		private string _namespace = "Test";

		private WebPath webPath;

		public SourceBuilder(string _namespace, WebPath webPath)
		{
			this._namespace = _namespace;
			this.webPath = webPath;
		}

		public string BackupToQuery(string tablename, WebPath webPath)
		{
			MapperProvider defaultProvider = MapperProvider.DefaultProvider;
			string str = string.Concat("SELECT * FROM ", tablename);
			string dataSetQuery = SourceBuilder.GetDataSetQuery(defaultProvider.QueryForValueSet(str, null, CommandType.Text).Tables[0], tablename);
			File.WriteAllText(webPath.Path, dataSetQuery);
			return string.Concat(new string[] { "<a href='", webPath.AbsolutePath, "'>", tablename, "</a>" });
		}

		public string BuildCommand(string tablename)
		{
			MapperProvider defaultProvider = MapperProvider.DefaultProvider;
			string pascalCase = tablename.Substring(3);
			pascalCase = SourceBuilder.ToPascalCase(pascalCase);
			string str = string.Concat("SELECT * FROM ", tablename);
			ValueSet valueSet = defaultProvider.QueryForValueSet(str, null, CommandType.Text);
			str = string.Concat("select object_id from sys.all_objects where type_desc='USER_TABLE' and name='", tablename, "'");
			string str1 = defaultProvider.QueryForScalar(str, null, CommandType.Text).ToString();
			str = string.Concat("select * from sys.index_columns where object_id='", str1, "'");
			ValueTable item = defaultProvider.QueryForValueSet(str, null, CommandType.Text).Tables[0];
			List<string> strs = new List<string>();
			foreach (ValueRow row in item.Rows)
			{
				str = string.Concat(new object[] { "select name from sys.columns where object_id='", str1, "' AND column_id='", row["column_id"], "'" });
				string str2 = defaultProvider.QueryForScalar(str, null, CommandType.Text).ToString();
				strs.Add(str2);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<mapper>\r\n\t<commands>\r\n");
			stringBuilder.AppendFormat("\t\t<post id=\"{0}.Create\">\r\n\t\t", pascalCase);
			stringBuilder.Append("<![CDATA[\r\n\t\t\t");
			stringBuilder.AppendFormat("INSERT INTO {0}", tablename);
			stringBuilder.Append("(");
			int num = 0;
			foreach (ValueColumn column in valueSet.Tables[0].Columns)
			{
				stringBuilder.AppendFormat("{0}{1}", (num > 0 ? ", " : " "), column.ColumnName);
				num++;
			}
			stringBuilder.AppendFormat(" )\r\n\t\t\tVALUES(", Array.Empty<object>());
			num = 0;
			foreach (ValueColumn valueColumn in valueSet.Tables[0].Columns)
			{
				stringBuilder.AppendFormat("{0}#{{{1}}}", (num > 0 ? ", " : " "), SourceBuilder.ToPascalCase(valueColumn.ColumnName));
				num++;
			}
			stringBuilder.AppendFormat(" )", Array.Empty<object>());
			stringBuilder.Append("\r\n\t\t\t]]>\r\n");
			stringBuilder.Append("\t\t</post>\r\n\r\n");
			stringBuilder.AppendFormat("\t\t<get id=\"{0}.Get\">\r\n\t\t", pascalCase);
			stringBuilder.Append("<![CDATA[\r\n\t\t\t");
			stringBuilder.AppendFormat("SELECT ", Array.Empty<object>());
			num = 0;
			foreach (ValueColumn column1 in valueSet.Tables[0].Columns)
			{
				stringBuilder.AppendFormat("{0}{1}", (num > 0 ? ", " : string.Empty), column1.ColumnName);
				num++;
			}
			stringBuilder.AppendFormat(" FROM {0} \r\n\t\t\tWHERE ", tablename);
			num = 0;
			foreach (string str3 in strs)
			{
				stringBuilder.AppendFormat("{0}{1}=@{2}", (num > 0 ? " AND " : string.Empty), str3, SourceBuilder.ToPascalCase(str3));
				num++;
			}
			stringBuilder.Append("\r\n\t\t\t]]>\r\n");
			stringBuilder.Append("\t\t</get>\r\n\r\n");
			stringBuilder.AppendFormat("\t\t<get id=\"{0}.GetList\">\r\n\t\t", pascalCase);
			stringBuilder.Append("<![CDATA[\r\n\t\t\t");
			stringBuilder.AppendFormat("SELECT ", Array.Empty<object>());
			num = 0;
			foreach (ValueColumn valueColumn1 in valueSet.Tables[0].Columns)
			{
				stringBuilder.AppendFormat("{0}{1}", (num > 0 ? ", " : string.Empty), valueColumn1.ColumnName);
				num++;
			}
			stringBuilder.AppendFormat(" FROM {0} \r\n\t\t\tWHERE ", tablename);
			num = 0;
			foreach (string str4 in strs)
			{
				stringBuilder.AppendFormat("{0}{1}=@{2}", (num > 0 ? " AND " : string.Empty), str4, SourceBuilder.ToPascalCase(str4));
				num++;
			}
			stringBuilder.Append("\r\n\t\t\t]]>\r\n");
			stringBuilder.Append("\t\t</get>\r\n\r\n");
			stringBuilder.AppendFormat("\t\t<patch id=\"{0}.Update\">\r\n\t\t\t", pascalCase);
			stringBuilder.Append("<![CDATA[\r\n\t\t\t");
			stringBuilder.AppendFormat("UPDATE {0} SET \r\n\t\t\t", tablename);
			num = 0;
			foreach (ValueColumn column2 in valueSet.Tables[0].Columns)
			{
				stringBuilder.AppendFormat("{0}{1}=@{2}", (num > 0 ? ", " : string.Empty), column2.ColumnName, SourceBuilder.ToPascalCase(column2.ColumnName));
				stringBuilder.AppendFormat("\r\n\t\t\t", Array.Empty<object>());
				num++;
			}
			stringBuilder.AppendFormat("WHERE ", Array.Empty<object>());
			num = 0;
			foreach (string str5 in strs)
			{
				stringBuilder.AppendFormat("{0}{1}=@{2}", (num > 0 ? " AND " : string.Empty), str5, SourceBuilder.ToPascalCase(str5));
				num++;
			}
			stringBuilder.Append("\r\n\t\t\t]]>\r\n");
			stringBuilder.Append("\t\t</patch>\r\n\r\n");
			stringBuilder.AppendFormat("\t\t<delete id=\"{0}.Delete\">\r\n\t\t\t", pascalCase);
			stringBuilder.Append("<![CDATA[\r\n\t\t\t");
			stringBuilder.AppendFormat("DELETE FROM {0} WHERE ", tablename);
			num = 0;
			foreach (string str6 in strs)
			{
				stringBuilder.AppendFormat("{0}{1}=@{2}", (num > 0 ? " AND " : string.Empty), str6, SourceBuilder.ToPascalCase(str6));
				num++;
			}
			stringBuilder.Append("\r\n\t\t\t]]>\r\n");
			stringBuilder.Append("\t\t</delete>\r\n\r\n");
			stringBuilder.Append("\t</commands>\r\n\r\n");
			stringBuilder.Append("\t<matches>\r\n");
			stringBuilder.Append("\t</matches>\r\n");
			stringBuilder.Append("</mapper>");
			return stringBuilder.ToString();
		}

		public string BuildController(string tablename)
		{
			string pascalCase = tablename.Substring(3);
			pascalCase = SourceBuilder.ToPascalCase(pascalCase);
			string str = "\r\nusing using Test.Core;\r\nusing System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Web;\r\nusing System.Web.Mvc;\r\n\r\nnamespace [SOLUTION].Controllers\r\n{\r\n    public class [ENTITY]Controller : BaseController\r\n    {\r\n        [HttpGet]\r\n        public ActionResult Write([ENTITY]SearchModel search)\r\n        {\r\n        }\r\n\r\n        [HttpPost]\r\n        public ActionResult Write([ENTITY]SearchModel search)\r\n        {\r\n            search.SetSort();\r\n            search.UserID = _User.UserID;\r\n\r\n            search.TotalCount = [PARAM]Repository.Get[ENTITY]Count(search);\r\n            ViewData[\"List\"] = [PARAM]Repository.Get[ENTITY]List(search);\r\n\r\n            ViewData[\"Search\"] = search;\r\n            return View();\r\n         \r\n        }\r\n\r\n        public ActionResult Read(string [PARAM]ID)\r\n        {\r\n            var [PARAM] = [PARAM]Repository.Get[ENTITY]([PARAM]ID);\r\n            \r\n            ViewData[\"[ENTITY]\"] = [PARAM];\r\n\r\n            return View();\r\n        }\r\n\r\n        [Authorize]\r\n        [HttpGet]\r\n        public ActionResult Write(string [PARAM]ID)\r\n        {\r\n            [PARAM]Entity [PARAM];\r\n            if (string.IsNullOrEmpty([PARAM]ID))\r\n            {\r\n                [PARAM] = new [PARAM]Entity();\r\n                [PARAM].[ENTITY]ID = Sequence.Generate();\r\n            }\r\n            else\r\n            {\r\n                [PARAM] = [PARAM]Repository.Get[ENTITY]([PARAM]ID);\r\n\r\n                if ([PARAM].RegID != _User.UserID && _User.User.AuthType != AuthType.ADMIN)\r\n                {\r\n                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);\r\n                }\r\n            }\r\n\r\n            ViewData[\"[ENTITY]\"] = [PARAM];\r\n\r\n            return View();\r\n        }\r\n    \r\n        [Authorize]\r\n        [HttpPost]\r\n        public ActionResult Write([ENTITY]Entity [PARAM])\r\n        {\r\n            var [PARAM] = [PARAM]Repository.Get[ENTITY]([PARAM].[ENTITY]ID);\r\n\r\n            if([PARAM] == null)\r\n            {\r\n                [PARAM].RegID = _User.UserID;\r\n                [PARAM]Repository.Create[ENTITY]([PARAM]);\r\n            }\r\n            else if (existEntity.RegID == _User.UserID || _User.User.AuthType == AuthType.ADMIN)\r\n            {\r\n                [PARAM]Repository.Update[ENTITY]([PARAM]);\r\n            }\r\n            else\r\n            {\r\n                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);\r\n            }\r\n\r\n            return RedirectToAction(\"List\");\r\n        }\r\n\r\n        [Authorize]\r\n        [HttpPost]\r\n        public ActionResult Delete(string [PARAM]ID )\r\n        {\r\n            var [PARAM] = [PARAM]Repository.Get[ENTITY]([PARAM]ID);\r\n\r\n            if ([PARAM].RegID == _User.UserID || _User.AuthType == AuthType.Admin)\r\n            {\r\n                [PARAM]Repository.Delete[ENTITY]([PARAM]ID);\r\n                \r\n            }\r\n            else\r\n            {\r\n                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);\r\n            }\r\n\r\n            return RedirectToAction(\"List\");\r\n        }\r\n    }\r\n}\r\n\r\n".Replace("[SOLUTION]", this._namespace).Replace("[ENTITY]", pascalCase);
			char chr = pascalCase[0];
			return str.Replace("[PARAM]", string.Concat(chr.ToString().ToLower(), pascalCase.Substring(1)));
		}

		public string BuildEntity(string tablename)
		{
			MapperProvider defaultProvider = MapperProvider.DefaultProvider;
			string str = string.Concat("SELECT * FROM ", tablename);
			ValueSet valueSet = defaultProvider.QueryForValueSet(str, null, CommandType.Text);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("using Test.Core;");
			stringBuilder.AppendLine("using System;");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(string.Concat("namespace ", this._namespace));
			stringBuilder.AppendLine("{");
			str = string.Concat("select object_id from sys.all_objects where type_desc='USER_TABLE' and name='", tablename, "'");
			string str1 = defaultProvider.QueryForScalar(str, null, CommandType.Text).ToString();
			List<string> strs = new List<string>();
			str = string.Concat("select * from sys.columns where object_id='", str1, "'");
			ValueTable item = defaultProvider.QueryForValueSet(str, null, CommandType.Text).Tables[0];
			str = string.Concat("select * from sys.index_columns where object_id='", str1, "'");
			ValueTable valueTable = defaultProvider.QueryForValueSet(str, null, CommandType.Text).Tables[0];
			List<string> strs1 = new List<string>();
			foreach (ValueRow row in valueTable.Rows)
			{
				str = string.Concat(new object[] { "select name from sys.columns where object_id='", str1, "' AND column_id='", row["column_id"], "'" });
				string str2 = defaultProvider.QueryForScalar(str, null, CommandType.Text).ToString();
				strs1.Add(str2);
			}
			stringBuilder.AppendLine(string.Format("\tpublic class {0}Entity : BaseEntity", this.GetCName(tablename, "TB_")));
			stringBuilder.AppendLine("\t{");
			foreach (ValueColumn column in valueSet.Tables[0].Columns)
			{
				ValueRow valueRow = item.Rows.FirstOrDefault<ValueRow>((ValueRow p) => p["name"].ToString() == column.ColumnName);
				string str3 = "string";
				string str4 = column.DataType.ToString();
				if (str4 == "System.Int32")
				{
					str3 = "int";
				}
				else if (str4 == "System.Int64")
				{
					str3 = "long";
				}
				else if (str4 == "System.Double")
				{
					str3 = "double";
				}
				else if (str4 == "System.Single")
				{
					str3 = "float";
				}
				else if (str4 == "System.DateTime")
				{
					str3 = "DateTime";
				}
				else
				{
					str3 = (str4 == "System.Boolean" ? "bool" : "string");
				}
				stringBuilder.AppendFormat(string.Concat("\t\t[Column(\"", column.ColumnName, "\", "), Array.Empty<object>());
				str = string.Concat("select name from sys.types where system_type_id='", valueRow["system_type_id"], "'");
				string name = defaultProvider.QueryForScalar(str, null, CommandType.Text).ToString();
				MemberInfo[] members = typeof(SqlDataType).GetMembers();
				for (int i = 0; i < (int)members.Length; i++)
				{
					MemberInfo memberInfo = members[i];
					if (memberInfo.Name.ToLower() == name)
					{
						name = memberInfo.Name;
					}
				}
				stringBuilder.AppendFormat(", SqlDataType.{0}", name);
				if ((!(bool)valueRow["is_nullable"] || column.Unique || strs1.Contains(column.ColumnName)) && strs1.Contains(column.ColumnName))
				{
					stringBuilder.AppendFormat("SqlColumnType.PrimaryKey", Array.Empty<object>());
				}
				stringBuilder.AppendLine(")]");
				stringBuilder.AppendLine(string.Format("\t\tpublic {0} {1} {{ get; set; }}", str3, this.GetCName(column.ColumnName, "")));
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine("\t}");
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}

		public string BuildMapper(string tablename)
		{
			string str = "\r\nusing Test.Core;\r\nusing System;\r\nusing System.Collections.Generic;\r\n\r\nnamespace [SOLUTION]\r\n{\r\n    public interface I[ENTITY]Mapper : IMapper\r\n    {\r\n        void Create([ENTITY]Entity [PARAM]);\r\n        int Update([ENTITY]Entity [PARAM]);\r\n        int Delete(Sequence [PARAM]Id);\r\n\r\n        [ENTITY]Entity Get(Sequence [PARAM]Id);\r\n        List<[ENTITY]Entity> GetList(SearchVo search, RangeVo range = null);\r\n        [Name(\"[ENTITY].GetList\")]\r\n        int GetListCount(SearchVo search);\r\n    }\r\n}\r\n".Trim();
			string pascalCase = tablename.Substring(3);
			pascalCase = SourceBuilder.ToPascalCase(pascalCase);
			string str1 = str.Replace("[SOLUTION]", this._namespace).Replace("[ENTITY]", pascalCase);
			char chr = pascalCase[0];
			return str1.Replace("[PARAM]", string.Concat(chr.ToString().ToLower(), pascalCase.Substring(1)));
		}

		public string BuildService(string tablename)
		{
			string pascalCase = tablename.Substring(3);
			pascalCase = SourceBuilder.ToPascalCase(pascalCase);
			string str = "\r\nusing Test.Core;\r\nusing System;\r\nusing System.Linq;\r\nusing System.Collections.Generic;\r\nusing System.Transactions;\r\n\r\nnamespace [SOLUTION]\r\n{\r\n    public class [ENTITY]Service : BaseService\r\n    {\r\n        public I[ENTITY]Mapper mapper[ENTITY] = null;\r\n\r\n        public int Create([ENTITY]Entity [PARAM])\r\n        {\r\n            mapper[ENTITY].Create([PARAM]);\r\n        }\r\n\r\n        public int Update([ENTITY]Entity [PARAM])\r\n        {\r\n            return mapper[ENTITY].Update([PARAM]);\r\n        }\r\n\r\n        public int Delete(Sequence [PARAM]Id)\r\n        {\r\n            return mapper[ENTITY].Delete([PARAM]Id);\r\n        }\r\n\r\n\r\n        public [ENTITY]Entity Get(Sequence [PARAM]Id)\r\n        {\r\n            return mapper[ENTITY].Get([PARAM]Id);\r\n        }\r\n\r\n        public List<[ENTITY]Entity> GetList(SearchVo search, bool containCount = true)\r\n        {\r\n            search.SetSort();\r\n            if (containCount)\r\n            {\r\n                search.TotalCount = mapper[ENTITY].GetListCount(search);\r\n            }\r\n            return mapper[ENTITY].GetList(search, search.GetRange());\r\n        }\r\n    }\r\n}\r\n".Replace("[SOLUTION]", this._namespace).Replace("[ENTITY]", pascalCase);
			char chr = pascalCase[0];
			return str.Replace("[PARAM]", string.Concat(chr.ToString().ToLower(), pascalCase.Substring(1)));
		}

		public void Generate()
		{
			ValueTable valueTable = MapperProvider.DefaultProvider.QueryForValueTable("select * from sys.all_objects where type_desc='USER_TABLE' and object_id>0", null, CommandType.Text);
			string path = this.webPath.Path;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (!Directory.Exists(string.Concat(path, "Command/")))
			{
				Directory.CreateDirectory(string.Concat(path, "Command/"));
			}
			if (!Directory.Exists(string.Concat(path, "Entity/")))
			{
				Directory.CreateDirectory(string.Concat(path, "Entity/"));
			}
			if (!Directory.Exists(string.Concat(path, "Mapper/")))
			{
				Directory.CreateDirectory(string.Concat(path, "Mapper/"));
			}
			if (!Directory.Exists(string.Concat(path, "Service/")))
			{
				Directory.CreateDirectory(string.Concat(path, "Service/"));
			}
			if (!Directory.Exists(string.Concat(path, "Controller/")))
			{
				Directory.CreateDirectory(string.Concat(path, "Controller/"));
			}
			foreach (ValueRow row in valueTable.Rows)
			{
				string item = row["name"] as string;
				string pascalCase = item.Substring(3);
				pascalCase = SourceBuilder.ToPascalCase(pascalCase);
				File.WriteAllText(string.Concat(path, "Command/", pascalCase, ".xml"), this.BuildCommand(item));
				File.WriteAllText(string.Concat(path, "Entity/", pascalCase, "Entity.cs"), this.BuildEntity(item));
				File.WriteAllText(string.Concat(path, "Mapper/I", pascalCase, "Mapper.cs"), this.BuildMapper(item));
				File.WriteAllText(string.Concat(path, "Service/", pascalCase, "Service.cs"), this.BuildService(item));
				File.WriteAllText(string.Concat(path, "Controller/", pascalCase, "Controller.cs"), this.BuildController(item));
			}
		}

		private string GetCName(string name, string prefix = "")
		{
			name = name.ToLower().Substring(prefix.Length);
			char chr = name[0];
			name = string.Concat(chr.ToString().ToUpper(), name.Substring(1));
			foreach (Match match in Regex.Matches(name, "_[a-z]"))
			{
				name = name.Replace(match.Value, match.Value.Substring(1).ToUpper());
			}
			return name;
		}

		public static string GetDataSetQuery(ValueTable dt, string tablename)
		{
			string str;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ValueRow row in dt.Rows)
			{
				stringBuilder.AppendFormat("INSERT INTO {0} VALUES(", tablename);
				foreach (ValueColumn column in dt.Columns)
				{
					str = (column.DataType != typeof(DateTime) ? row[column.ColumnName].ToString().Replace("'", "''") : SourceBuilder.GetDBDateString((DateTime)row[column.ColumnName]));
					stringBuilder.AppendFormat("'{0}',", str);
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.AppendFormat(")\n", Array.Empty<object>());
			}
			return stringBuilder.ToString();
		}

		private static string GetDBDateString(DateTime dt)
		{
			if (dt == DateTime.MinValue)
			{
				return "null";
			}
			int num = (dt.Hour > 12 ? dt.Hour - 12 : dt.Hour);
			string str = (dt.Hour > 12 ? "PM" : "AM");
			object[] month = new object[] { dt.Month, dt.Day, dt.Year, num.ToString().PadLeft(2, '0'), null, null, null, null };
			int minute = dt.Minute;
			month[4] = minute.ToString().PadLeft(2, '0');
			minute = dt.Second;
			month[5] = minute.ToString().PadLeft(2, '0');
			minute = dt.Millisecond;
			month[6] = minute.ToString().PadLeft(3, '0');
			month[7] = str;
			return string.Format("{0} {1} {2} {3}:{4}:{5}:{6}{7}", month);
		}

		private static string ToPascalCase(string str)
		{
			string[] strArrays = str.Split(new char[] { '\u005F' });
			string empty = string.Empty;
			string[] strArrays1 = strArrays;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				string str1 = strArrays1[i];
				if (str1.Length > 1)
				{
					char chr = str1[0];
					empty = string.Concat(empty, chr.ToString().ToUpper(), str1.Substring(1).ToLower());
				}
				else
				{
					empty = string.Concat(empty, str1);
				}
			}
			return empty;
		}
	}
}