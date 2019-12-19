using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using Notice.Core;
using Notice.Data.Core;
    

namespace Notice.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected static IEnumerable<Assembly> GetAssemblies()
        {
            var stack = new Stack<Assembly>();
            stack.Push(Assembly.GetExecutingAssembly());
            do
            {
                var asm = stack.Pop();
                if (asm.FullName.StartsWith("Notice."))
                {
                    yield return asm;
                }

                foreach (var reference in asm.GetReferencedAssemblies())
                {
                    if (reference.FullName.StartsWith("Notice."))
                    {
                        stack.Push(Assembly.Load(reference));
                    }
                }
            }
            while (stack.Count > 0);
        }

        static ILog logger;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DateTime now = DateTime.Now;

            Assembly assemObj = Assembly.GetExecutingAssembly();
            Version v = assemObj.GetName().Version;

            Config.SetSuffix(ConfigurationManager.AppSettings["SystemChar"]);

            LogManager.AddSetting(new LogSetting {
                Name = "Notice Log",
                Version = v.ToString(),
                Level = LogLevel.Info,
                LogPath = Server.MapPath("~/Log/"),
                Type = LoggingType.File });

            var assemblies = GetAssemblies().Distinct();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("FileRoot", Server.MapPath("FileRoot"));

            InitializeGlobal.Initilize(assemblies, DateTime.UtcNow.Ticks - 636266462228696765, dic);

            logger = LogManager.GetLogger();
            logger.Info("Application", "start-" + (DateTime.Now - now).TotalSeconds);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            if (ex is HttpException) {
                return;
            }

            logger.Error("Error", ex.Message, ex);
        }
    }
}
