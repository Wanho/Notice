using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Test.Core
{
	public class ModelLoader
	{
		private ILog logger = LogManager.GetLogger("Test.ModelLoader");

		internal int interval;

		internal Timer timer;

		private int maxTryCount = 5;

		private int tryCount;

		internal Type type;

		private IModelLoader loader;

		public static Dictionary<Type, ModelLoader> Loaders;

		static ModelLoader()
		{
			ModelLoader.Loaders = new Dictionary<Type, ModelLoader>();
		}

		internal ModelLoader(Type type, ModelLoaderAttribute attr)
		{
			this.type = type;
			object obj = Activator.CreateInstance(type);
			this.loader = obj as IModelLoader;
			if (this.loader == null)
			{
				throw new NotImplementedException(string.Concat(type.FullName, " not Implemented IModelLoader interface"));
			}
			BaseMapperService baseMapperService = obj as BaseMapperService;
			if (baseMapperService != null)
			{
				baseMapperService.SetModelLoader(this);
			}
			this.interval = attr.Interval;
			this.timer = new Timer(new TimerCallback(this.TimerCallback), null, -1, -1);
			this.maxTryCount = attr.TryCount;
		}

		public static void AddModelLoader(Type type, int interval = -1)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
			ModelLoaderAttribute customAttribute = typeInfo.GetCustomAttribute<ModelLoaderAttribute>(false);
			if (customAttribute != null)
			{
				if (typeInfo.DeclaredConstructors.FirstOrDefault<ConstructorInfo>((ConstructorInfo p) => p.GetParameters().Length == 0) == null)
				{
					throw new Exception(string.Concat(type.FullName, " not exist default constructor"));
				}
				ModelLoader modelLoader = new ModelLoader(type, customAttribute);
				ModelLoader.Loaders.Add(type, modelLoader);
				if (interval == -1)
				{
					interval = customAttribute.Interval;
				}
				modelLoader.Start(interval, true);
			}
		}

		internal void Start(int interval, bool load = false)
		{
			this.interval = interval;
			if (load)
			{
				this.TimerCallback(null);
				return;
			}
			this.timer.Change(interval, -1);
		}

		internal void TimerCallback(object state)
		{
			try
			{
				DateTime now = DateTime.Now;
				if (this.loader.Load())
				{
                    ILog log = this.logger;
					string fullName = this.type.FullName;
					TimeSpan timeSpan = DateTime.Now - now;
					log.Info(string.Concat(fullName, "  is loaded :", timeSpan.TotalSeconds), (LogInfo)null, null);
				}
				if (!ModelLoaderAttribute.forceNoTimer && this.interval > 0)
				{
					this.timer.Change(this.interval, -1);
				}
				this.tryCount = 0;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (this.tryCount < this.maxTryCount)
				{
					this.tryCount++;
				}
				this.logger.Error(string.Concat(this.type.FullName, "  is fail to loading. tryCount is ", this.tryCount), exception);
				if (this.tryCount == this.maxTryCount || ModelLoaderAttribute.forceNoTimer)
				{
					this.logger.Error(string.Concat(this.type.FullName, "  be stopped."), exception);
					throw;
				}
				else
				{
					this.timer.Change(1000, -1);
				}
			}
		}
	}
}