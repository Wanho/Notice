using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Notice.Core
{
    public abstract class InitializeBaseCode
    {
        private static ILog logger;

        static InitializeBaseCode()
        {
            InitializeBaseCode.logger = LogManager.GetLogger("Notice.Code");
        }

        protected InitializeBaseCode()
        {
        }

        public static void Initialize(IEnumerable<Assembly> assemblies, long startSeq = -1L)
        {
            InitializeBaseCode.logger.Debug("Code Init", (LogInfo)null, null);
            assemblies = assemblies.Concat<Assembly>((IEnumerable<Assembly>)(new Assembly[] { typeof(BaseCode).GetTypeInfo().Assembly }));
           
            foreach (Assembly assembly in assemblies)
            {
                InitializeBaseCode.logger.Debug(string.Concat("Code Load ", assembly.FullName), (LogInfo)null, null);
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < (int)types.Length; i++)
                {
                    Type type = types[i];
                    string name = type.Name;
                    if (type.GetTypeInfo().IsSubclassOf(typeof(BaseCode)) && type.GetTypeInfo().GetCustomAttribute<Custom_NotInitTypeAttribute>(false) == null)
                    {
                        type.GetMethod("Init", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Invoke(null, null);
                    }
                }
            }
        }

        
    }
}
