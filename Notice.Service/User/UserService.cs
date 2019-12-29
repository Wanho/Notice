using Notice.Data;
using Notice.Data.Core;
using Notice.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notice.Service
{
    public class UserService
    {
        public class UserDbContext : DbContext
        {
            public UserDbContext() : base("name=DefaultConnection")
            {
                Database.SetInitializer<DataContext>(null);
            }

            public DbSet<UserModel> Users { get; set; }
        }

        UserDbContext dbContext;

        public UserService()
        {
            dbContext = new UserDbContext();
        }

        public UserModel GetById(string userID)
        {
            return dbContext.Users.Find(userID);
        }

        public bool Verification(string userID, string password)
        {
            UserModel user = GetById(userID);

            if(user != null)
            {
                var result = HashWithSlat.VerifyHashedPassword(user.PasswordHash, password);
                if (result == PasswordVerificationResult.Success)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public void CreateUser(UserModel user)
        {

        }

    }
}
