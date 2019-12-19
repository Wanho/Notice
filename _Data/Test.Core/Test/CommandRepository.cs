using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

namespace Test.Core
{
	internal class CommandRepository
	{
		private Dictionary<string, CommandItem> commands = new Dictionary<string, CommandItem>();

		internal static CommandRepository Instance;

		private static ILog logger;

		internal CommandItem this[string commandId]
		{
			get
			{
				if (!this.commands.ContainsKey(commandId))
				{
					return null;
				}
				return this.commands[commandId];
			}
		}

		static CommandRepository()
		{
			CommandRepository.logger = LogManager.GetLogger("Test.CommandRepository");
		}

		public CommandRepository()
		{
		}

		internal static void AddCommand(string commandsXml)
		{
			object value;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(commandsXml);
			XmlNode xmlNodes = xmlDocument.SelectSingleNode("/commands");
			if (xmlNodes == null)
			{
				return;
			}
			Dictionary<string, MatchItem> strs = new Dictionary<string, MatchItem>();
			foreach (XmlElement xmlElement in xmlNodes.SelectNodes("commands/*"))
			{
				CommandVerb name = (CommandVerb)((Code<CommandVerb>)xmlElement.Name);
				if (name == null)
				{
					throw new Exception(string.Concat(xmlElement.Name, " wrong CommandVerb"));
				}
				if (name == null)
				{
					continue;
				}
				XmlAttribute itemOf = xmlElement.Attributes["type"];
				if (itemOf != null)
				{
					value = itemOf.Value;
				}
				else
				{
					value = null;
				}
				if (value == null)
				{
					value = "Text";
				}
				string str = (string)value;
				CommandItem commandItem = new CommandItem()
				{
					Id = xmlElement.Attributes["id"].Value,
					Type = EnumExtension.Parse<Test.Core.CommandType>(str),
					Verb = name,
					Text = xmlElement.ChildNodes[0].InnerText.Trim()
				};
				commandItem.Parse();
				foreach (XmlElement xmlElement1 in xmlElement.SelectNodes("*"))
				{
					MatchItem matchItem = ((MatchType)((Code<MatchType>)xmlElement1.Name)).CreateMatchItem();
					if (matchItem == null)
					{
						throw new Exception(string.Concat(commandItem.Id, ".", xmlElement1.Name, " wrong MatchType"));
					}
					xmlElement1.ToVo<MatchItem>(matchItem);
					if (matchItem.Type == null)
					{
						continue;
					}
					commandItem.Matches.Add(matchItem);
				}
				foreach (MatchItem value1 in strs.Values)
				{
					commandItem.Matches.Add(value1);
				}
				if (!CommandRepository.Instance.commands.ContainsKey(commandItem.Id))
				{
					CommandRepository.Instance.commands.Add(commandItem.Id, commandItem);
				}
				else
				{
					CommandRepository.Instance.commands[commandItem.Id] = commandItem;
				}
			}
		}

		internal static void CreateRepository(string path)
		{
			string value;
			object obj;
			object value1;
			object obj1;
			string str;
			try
			{
				try
				{
					string[] files = Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories);
					CommandRepository commandRepository = new CommandRepository();
					string[] strArrays = files;
					for (int i = 0; i < (int)strArrays.Length; i++)
					{
						string str1 = strArrays[i];
						XmlDocument xmlDocument = new XmlDocument();
						FileStream fileStream = File.OpenRead(str1);
						xmlDocument.Load(fileStream);
						fileStream.Dispose();
						XmlNode xmlNodes = xmlDocument.SelectSingleNode("/mapper");
						if (xmlNodes != null)
						{
							XmlAttribute itemOf = xmlNodes.Attributes["provider"];
							if (itemOf != null)
							{
								value = itemOf.Value;
							}
							else
							{
								value = null;
							}
							string str2 = value;
							List<MatchItem> matchItems = new List<MatchItem>();
							foreach (XmlElement xmlElement in xmlNodes.SelectNodes("matches/*"))
							{
								MatchItem matchItem = ((MatchType)((Code<MatchType>)xmlElement.Name)).CreateMatchItem();
								if (matchItem == null)
								{
									throw new Exception(string.Concat(xmlElement.Name, " wrong MatchType"));
								}
								xmlElement.ToVo<MatchItem>(matchItem);
								matchItems.Add(matchItem);
							}
							foreach (XmlElement xmlElement1 in xmlNodes.SelectNodes("commands/*"))
							{
								CommandVerb name = (CommandVerb)((Code<CommandVerb>)xmlElement1.Name);
								if (name == null)
								{
									throw new Exception(string.Concat(xmlElement1.Name, " wrong CommandVerb"));
								}
								if (name == null)
								{
									continue;
								}
								XmlAttribute xmlAttribute = xmlElement1.Attributes["type"];
								if (xmlAttribute != null)
								{
									obj = xmlAttribute.Value;
								}
								else
								{
									obj = null;
								}
								if (obj == null)
								{
									obj = "Text";
								}
								string str3 = (string)obj;
								CommandItem commandItem = new CommandItem()
								{
									Id = xmlElement1.Attributes["id"].Value.Trim(),
									Type = EnumExtension.Parse<Test.Core.CommandType>(str3),
									Verb = name,
									Text = xmlElement1.ChildNodes[0].InnerText
								};
								XmlAttribute itemOf1 = xmlElement1.Attributes["provider"];
								if (itemOf1 != null)
								{
									value1 = itemOf1.Value;
								}
								else
								{
									value1 = null;
								}
								if (value1 == null)
								{
									value1 = str2;
								}
								commandItem.Provider = (string)value1;
								XmlAttribute xmlAttribute1 = xmlElement1.Attributes["timeout"];
								if (xmlAttribute1 != null)
								{
									obj1 = xmlAttribute1.Value;
								}
								else
								{
									obj1 = null;
								}
								if (obj1 == null)
								{
									obj1 = "0";
								}
								commandItem.Timeout = short.Parse((string)obj1);
								XmlAttribute itemOf2 = xmlElement1.Attributes["result"];
								if (itemOf2 != null)
								{
									str = itemOf2.Value;
								}
								else
								{
									str = null;
								}
								object nothing = (CommandResult)((Code<CommandResult>)str);
								if (nothing == null)
								{
									nothing = BaseCode<CommandResult>.Nothing;
								}
								commandItem.ResultType = (CommandResult)nothing;
								CommandItem commandItem1 = commandItem;
								commandItem1.Parse();
								foreach (XmlElement xmlElement2 in xmlElement1.SelectNodes("*"))
								{
									MatchItem matchItem1 = ((MatchType)((Code<MatchType>)xmlElement2.Name)).CreateMatchItem();
									if (matchItem1 == null)
									{
										throw new Exception(string.Concat(commandItem1.Id, ".", xmlElement2.Name, " wrong MatchType"));
									}
									xmlElement2.ToVo<MatchItem>(matchItem1);
									if (matchItem1.Type == null)
									{
										continue;
									}
									commandItem1.Matches.Add(matchItem1);
								}
								foreach (MatchItem matchItem2 in matchItems)
								{
									if (commandItem1.Matches.Find((MatchItem p) => p.Name == matchItem2.Name) != null)
									{
										continue;
									}
									commandItem1.Matches.Add(matchItem2);
								}
								if (!commandRepository.commands.ContainsKey(commandItem1.Id))
								{
									commandRepository.commands.Add(commandItem1.Id, commandItem1);
								}
								else
								{
									commandRepository.commands[commandItem1.Id] = commandItem1;
								}
							}
						}
					}
					CommandRepository.Instance = commandRepository;
					CommandRepository.logger.Info(string.Concat("resource loaded", Environment.NewLine, path), (LogInfo)null, null);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (CommandRepository.Instance != null)
					{
						CommandRepository.logger.Error("reloading fail, retry", exception);
						CommandRepository.CreateRepository(path);
					}
					else
					{
						CommandRepository.logger.Fatal("loading fail", exception);
						Thread.Sleep(1000);
					}
					throw exception;
				}
			}
			finally
			{
				(new Thread(() => {
					(new FileSystemWatcher()
					{
						Path = path,
						IncludeSubdirectories = true,
						EnableRaisingEvents = true
					}).WaitForChanged(WatcherChangeTypes.Changed);
					Thread.Sleep(3000);
					CommandRepository.CreateRepository(path);
				})).Start();
			}
		}

		private static SqlDbType GetSqlDbType(string type)
		{
			SqlDbType sqlDbType;
			if (Enum.TryParse<SqlDbType>(type, out sqlDbType))
			{
				sqlDbType = SqlDbType.VarChar;
			}
			return sqlDbType;
		}
	}
}