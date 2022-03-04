using System;
using System.Collections.Generic;
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
            FileTypeCode.AddRoot("FileRoot", "/FileRoot/", initParam["FileRoot"]);

            InitializeBaseCode.Initialize(assemblies, startSeq);
        }
    }

}
