using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity.EntityFramework;

namespace Notice.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }

}
