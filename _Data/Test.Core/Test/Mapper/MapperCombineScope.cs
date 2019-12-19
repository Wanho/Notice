using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Test.Core
{
	public class MapperCombineScope : IDisposable
	{
		internal bool noCombine;

		internal int refCount;

		private int completeCount;

		private int disposeCount;

		private BaseMapper mapper;

		private List<MapperCommand> commands = new List<MapperCommand>();

		private MapperCombineState state = MapperCombineState.Ready;

		private static ILog logger;

		static MapperCombineScope()
		{
			MapperCombineScope.logger = LogManager.GetLogger("Test.MapperCombine");
		}

		internal MapperCombineScope(BaseMapper mapper)
		{
			this.mapper = mapper;
		}

		public MapperCommand AddCommand(string commandId, params object[] source)
		{
			MapperCommand command = MapperCommand.GetCommand(this.mapper.Provider, commandId, source);
			return this.AddCommand(command);
		}

		public MapperCommand AddCommand(MapperCommand command)
		{
			this.commands.Add(command);
			if (this.noCombine)
			{
				command.Query();
			}
			return command;
		}

		public MapperCommand AddSpCommand(string sptext, params object[] source)
		{
			MapperCommand spCommand = MapperCommand.GetSpCommand(this.mapper.Provider, sptext, source);
			return this.AddCommand(spCommand);
		}

		public MapperCommand AddTextCommand(string text, params object[] source)
		{
			MapperCommand textCommand = MapperCommand.GetTextCommand(this.mapper.Provider, text, source);
			return this.AddCommand(textCommand);
		}

		private void CheckState()
		{
			if (this.state == MapperCombineState.Complete)
			{
				throw new Exception("already completed context");
			}
			if (this.state == MapperCombineState.Dispose)
			{
				throw new Exception("already disposed context");
			}
		}

		public void Complete()
		{
			this.completeCount++;
			if (this.refCount == this.completeCount)
			{
				this.Run();
				this.state = MapperCombineState.Complete;
				this.mapper.EndCombineScope();
			}
		}

		public void Dispose()
		{
			this.disposeCount++;
			if (this.refCount == this.disposeCount)
			{
				if (this.completeCount < this.disposeCount)
				{
					this.Run();
				}
				this.state = MapperCombineState.Dispose;
				this.mapper.EndCombineScope();
			}
		}

		public void Run()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MapperCommand command in this.commands)
			{
				stringBuilder.AppendLine(command.Text);
			}
			if (this.mapper.Provider.Logging)
			{
				MapperCombineScope.logger.Debug(this.mapper.GetType().Name, string.Concat(new object[] { "Run", this.commands.Count, "\r\n", stringBuilder.ToString() }), null, null);
			}
			if (this.commands.Count > 0 && !this.noCombine)
			{
				this.mapper.Provider.Query(stringBuilder.ToString(), null, CommandType.Text);
			}
		}
	}
}