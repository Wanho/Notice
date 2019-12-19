using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Kolon
{

    /// <summary>
    /// DataBase First Data ( 데이터 없을 경우 기본 생성 )
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbInitialize<T> : DropCreateDatabaseIfModelChanges<IdentityDbContext>
    {
        protected override void Seed(IdentityDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(context));

            const string name = "Admin";
            const string password = "password";

            if (!roleManager.RoleExists(name))
            {
                var roleresult = roleManager.Create(new ApplicationRole(name));
            }

            var user = new ApplicationUser();
            user.UserName = name;
            var adminresult = userManager.Create(user, password);

            if (adminresult.Succeeded)
            {
                var result = userManager.AddToRole(user.Id, name);
            }
            base.Seed(context);
        }
    }
}
