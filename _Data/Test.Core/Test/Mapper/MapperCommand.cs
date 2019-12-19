using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Test.Core
{
	public sealed class MapperCommand
	{
		private MapperProvider provider;

		private string commandId;

		private List<ParameterSource> parameterSource;

		private static ILog logger;

		public static string Config
		{
			get;
			set;
		}

		public bool Logging { get; set; } = true;

		public MapperParameter Parameter { get; private set; } = new MapperParameter();

		public MapperProvider Provider
		{
			get
			{
				return this.provider;
			}
		}

		public CommandResult Result
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}

		public short Timeout
		{
			get;
			set;
		}

		public Test.Core.CommandType Type { get; set; } = Test.Core.CommandType.Text;

		static MapperCommand()
		{
			MapperCommand.logger = LogManager.GetLogger("Test");
		}

		public MapperCommand()
		{
			this.provider = MapperProvider.DefaultProvider;
		}

		public MapperCommand(string text)
		{
			this.provider = MapperProvider.DefaultProvider;
			this.Text = text;
		}

		public MapperCommand(MapperProvider provider)
		{
			this.provider = provider;
		}

		public MapperCommand(MapperProvider provider, string text)
		{
			this.provider = provider;
			this.Text = text;
		}

		public static void AddCommand(string commandsXml)
		{
			CommandRepository.AddCommand(commandsXml);
		}

		public void ApplyParameter()
		{
			CommandItem commandItem = new CommandItem()
			{
				Text = this.Text
			};
			commandItem.Parse();
			if (this.parameterSource == null)
			{
				this.parameterSource = new List<ParameterSource>();
				this.parameterSource.AddRange(this.provider.Parameter);
				foreach (MapperParameterItem value in this.Parameter.Values)
				{
					this.parameterSource.Add(new ParameterSource()
					{
						Name = value.ParameterName,
						Value = value.Value
					});
				}
			}
			this.Text = commandItem.GetParameterMatchText(this.parameterSource);
		}

		internal void BuildCommand()
		{
			if (this.Type == Test.Core.CommandType.StoredProcedure)
			{
				this.BuildSPCommand();
				return;
			}
			this.BuildTextCommand();
		}

		private void BuildSPCommand()
		{
			object value;
			string[] strArrays = Regex.Replace(Regex.Replace(this.Text, "[\\s]+", " "), " *, *", ",").Trim().Split(new char[] { ' ' });
			if ((int)strArrays.Length < 1)
			{
				throw new Exception("sentense is wrong");
			}
			this.Text = strArrays[0];
			if ((int)strArrays.Length > 1)
			{
				string[] strArrays1 = strArrays[1].Split(new char[] { ',' });
				for (int i = 0; i < (int)strArrays1.Length; i++)
				{
					string str = strArrays1[i];
					string[] strArrays2 = str.Split(new char[] { '=' });
					MapperParameterItem mapperParameterItem = this.Parameter.Add(strArrays2[0]);
					MapperParameterItem mapperParameterItem1 = mapperParameterItem;
					ParameterSource parameterSource = this.parameterSource.Find((ParameterSource p) => p.Name.MatchDataName(strArrays2[0]));
					if (parameterSource != null)
					{
						value = parameterSource.Value;
					}
					else
					{
						value = null;
					}
					mapperParameterItem1.Value = value;
					if (!strArrays2[1].StartsWith("@"))
					{
						mapperParameterItem.Direction = ParameterDirection.Input;
						mapperParameterItem.Value = strArrays2[1].Trim(new char[] { '\'' });
					}
					else if (strArrays2[1].LastIndexOf("<table>") > 0)
					{
						mapperParameterItem.IsTable = true;
						ValueTable dataTable = mapperParameterItem.Value.ToString().ToDataTable();
						dataTable.TableName = strArrays2[0];
						mapperParameterItem.Value = dataTable;
					}
					else if (strArrays2[1].LastIndexOf("<out>") <= 0)
					{
						mapperParameterItem.Direction = ParameterDirection.Input;
					}
					else
					{
						mapperParameterItem.Direction = ParameterDirection.Output;
					}
				}
			}
		}

		private void BuildTextCommand()
		{
			foreach (Match match in this.provider.DBParameterRegex.Matches(this.Text))
			{
				bool value = match.Value[0] == 'N';
				string str = match.Groups[2].Value;
				ParameterSource parameterSource = this.parameterSource.Find((ParameterSource p) => p.Name.MatchDataName(str));
				if (parameterSource == null || this.Parameter.ContainsKey(parameterSource.Name))
				{
					continue;
				}
				MapperParameterItem type = this.Parameter.Add(parameterSource.Name, parameterSource.Value);
				if (parameterSource.Value != null)
				{
					type.Type = parameterSource.Value.GetType();
				}
				if (parameterSource.Column != null)
				{
					type.SqlType = new SqlDataType?(parameterSource.Column.DataType);
				}
				else
				{
					this.Parameter[parameterSource.Name].Value = parameterSource.Value;
					if (!value)
					{
						continue;
					}
					type.SqlType = new SqlDataType?(SqlDataType.NVarChar);
				}
			}
		}

		private static List<ParameterSource> ConvertParameterSource(MapperProvider provider, object[] source)
		{
			if (source == null)
			{
				source = new object[0];
			}
			for (int i = 0; i < (int)source.Length; i++)
			{
				if (source[i] == null)
				{
					source[i] = new ParameterSource();
				}
			}
			List<ParameterSource> parameterSources = new List<ParameterSource>((int)source.Length);
			parameterSources.AddRange(provider.Parameter);
			object[] objArray = source;
			for (int j = 0; j < (int)objArray.Length; j++)
			{
				object obj = objArray[j];
				if (!(obj is ValueObject))
				{
					if (!(obj is Dictionary<string, object>))
					{
						goto Label1;
					}
					Dictionary<string, object> strs = obj as Dictionary<string, object>;
					Dictionary<string, object>.KeyCollection.Enumerator enumerator = strs.Keys.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							string current = enumerator.Current;
							parameterSources.Add(new ParameterSource()
							{
								Name = current,
								Value = strs[current]
							});
						}
						goto Label0;
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				else
				{
					System.Type type = obj.GetType();
					parameterSources.Add(new ParameterSource()
					{
						Name = type.Name,
						Value = type
					});
				}
			Label1:
				ParamterTypeAttribute customAttribute = obj.GetType().GetTypeInfo().GetCustomAttribute<ParamterTypeAttribute>();
				PropertyInfo[] properties = ((customAttribute == null ? obj.GetType() : customAttribute.ParamterType)).GetProperties();
				for (int k = 0; k < (int)properties.Length; k++)
				{
					PropertyInfo propertyInfo = properties[k];
					if (propertyInfo.GetCustomAttribute<IgnoreAttribute>() == null)
					{
						parameterSources.Add(new ParameterSource()
						{
							Name = propertyInfo.Name,
							Column = propertyInfo.GetCustomAttribute<ColumnAttribute>(),
							Value = propertyInfo.GetValue(obj)
						});
					}
				}
            Label0:;
			}
			return parameterSources;
		}

		public static MapperCommand GetCommand(string commandId, params object[] source)
		{
			CommandItem item = CommandRepository.Instance[commandId];
			if (item == null)
			{
				throw new Exception(string.Concat(commandId, " not exist mapper command"));
			}
			return MapperCommand.GetCommand((string.IsNullOrEmpty(item.Provider) ? MapperProvider.DefaultProvider : MapperProvider.CreateProvider(item.Provider)), item, source);
		}

		public static MapperCommand GetCommand(MapperProvider provider, string commandId, params object[] source)
		{
			CommandItem item = CommandRepository.Instance[commandId];
			if (item == null)
			{
				throw new Exception(string.Concat(commandId, " not exist mapper command"));
			}
			return MapperCommand.GetCommand(provider, item, source);
		}

		private static MapperCommand GetCommand(MapperProvider provider, CommandItem item, params object[] source)
		{
			if (provider == null)
			{
				provider = MapperProvider.DefaultProvider;
			}
			List<ParameterSource> parameterSources = MapperCommand.ConvertParameterSource(provider, source);
			MapperCommand mapperCommand = new MapperCommand(provider)
			{
				commandId = item.Id,
				parameterSource = parameterSources,
				Type = item.Type,
				Text = item.GetParameterMatchText(parameterSources),
				Result = item.ResultType,
				Timeout = item.Timeout
			};
			mapperCommand.BuildCommand();
			return mapperCommand;
		}

		public static MapperCommand GetSpCommand(MapperProvider provider, string text, params object[] source)
		{
			if (provider == null)
			{
				provider = MapperProvider.DefaultProvider;
			}
			List<ParameterSource> parameterSources = MapperCommand.ConvertParameterSource(provider, source);
			MapperCommand mapperCommand = new MapperCommand(provider)
			{
				parameterSource = parameterSources,
				Text = text,
				Type = Test.Core.CommandType.StoredProcedure
			};
			mapperCommand.BuildCommand();
			return mapperCommand;
		}

		public static MapperCommand GetTextCommand(MapperProvider provider, string text, params object[] source)
		{
			if (provider == null)
			{
				provider = MapperProvider.DefaultProvider;
			}
			List<ParameterSource> parameterSources = MapperCommand.ConvertParameterSource(provider, source);
			MapperCommand mapperCommand = new MapperCommand(provider)
			{
				parameterSource = parameterSources,
				Type = Test.Core.CommandType.Text,
				Text = text
			};
			mapperCommand.BuildCommand();
			return mapperCommand;
		}

		public static int Query(MapperProvider provider, string commandId, params object[] param)
		{
			return MapperCommand.GetCommand(provider, commandId, param).Query();
		}

		public int Query()
		{
			return (int)this.QueryForResult(CommandResult.None);
		}

		public object QueryForResult(CommandResult result = null)
		{
			object obj;
			if (result == null)
			{
				result = this.Result;
			}
			bool flag = false;
			try
			{
				try
				{
					string str = result;
					switch (str)
					{
						case "Set":
						{
							obj = this.provider.QueryForValueSet(this);
							break;
						}
						case "Table":
						{
							obj = this.provider.QueryForValueTable(this);
							break;
						}
						case "Row":
						{
							obj = this.provider.QueryForValueRow(this);
							break;
						}
						case "Scalar":
						{
							obj = this.provider.QueryForScalar(this);
							break;
						}
						default:
						{
							obj = (str == "None" || str == "Nothing" || str == "Output" ? this.provider.QueryForValueRow(this) : null);
							break;
						}
					}
				}
				catch (Exception exception)
				{
					flag = true;
					throw exception;
				}
			}
			finally
			{
				this.WriteDataCommand(result, flag);
			}
			return obj;
		}

		public static T QueryForScalar<T>(MapperProvider provider, string commandId, params object[] param)
		{
			return (T)MapperCommand.GetCommand(provider, commandId, param).QueryForScalar();
		}

		public object QueryForScalar()
		{
			return this.QueryForResult(CommandResult.Scalar);
		}

		public static ValueRow QueryForValueRow(MapperProvider provider, string commandId, params object[] param)
		{
			return MapperCommand.GetCommand(provider, commandId, param).QueryForValueRow();
		}

		public ValueRow QueryForValueRow()
		{
			return (ValueRow)this.QueryForResult(CommandResult.Row);
		}

		public static ValueSet QueryForValueSet(MapperProvider provider, string commandId, params object[] param)
		{
			return MapperCommand.GetCommand(provider, commandId, param).QueryForValueSet();
		}

		public ValueSet QueryForValueSet()
		{
			return (ValueSet)this.QueryForResult(CommandResult.Set);
		}

		public static ValueTable QueryForValueTable(MapperProvider provider, string commandId, params object[] param)
		{
			return MapperCommand.GetCommand(provider, commandId, param).QueryForValueTable();
		}

		public ValueTable QueryForValueTable()
		{
			return (ValueTable)this.QueryForResult(CommandResult.Table);
		}

		private void WriteDataCommand(string type, bool isError = false)
		{
			if (this.Logging)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(string.Concat("Command: ", this.commandId));
				stringBuilder.AppendLine("Parameter: ");
				foreach (string key in this.Parameter.Keys)
				{
					stringBuilder.AppendLine(string.Concat(new object[] { "name: ", key, " type: ", this.Parameter[key].Type, "/sqltype: ", this.Parameter[key].SqlType, " val: ", this.Parameter[key].Value }));
				}
				stringBuilder.AppendLine("Text: ");
				stringBuilder.AppendLine(this.Text);
				if (isError)
				{
					MapperCommand.logger.Debug(string.Concat("Query.", type), stringBuilder.ToString(), null, null);
					return;
				}
				MapperCommand.logger.Error(string.Concat("Query.", type), stringBuilder.ToString(), null, null);
			}
		}
	}
}