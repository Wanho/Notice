using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Notice.Core
{
    public static class InitializeGlobal
    {
        public static void Initilize(IEnumerable<Assembly> assemblies, long startSeq, Dictionary<string, string> initParam)
        {
            CodeFileType.AddRoot("Custom_FileRoot", "/FileRoot/", initParam["FileRoot"]);

            InitializeBaseCode.Initialize(assemblies, startSeq);
        }
    }

}
