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

            public DbSet<User> Users { get; set; }
        }

        UserDbContext dbContext;

        public UserService()
        {
            dbContext = new UserDbContext();
        }

        public User GetById(string userID)
        {
            return dbContext.Users.Find(userID);
        }

        public bool Verification(string userID, string password)
        {
            User user = GetById(userID);

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

        public UserModel GetUser(string id, string pass)
        {
            string strSql = @"SELECT id FROM TB_User WHERE Id = @id and password = @pass";

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", id));
            parameters.Add(new SqlParameter("@pass", pass));

            var rtnVal = dbContext.Database.SqlQuery<UserModel>(strSql, parameters.ToArray()).FirstOrDefault();

            return rtnVal;
        }

        public UserModel GetUser(string id)
        {
            string strSql = @"SELECT id FROM TB_User WHERE Id = @id";

            var rtnVal = dbContext.Database.SqlQuery<UserModel>(strSql, new SqlParameter("@id", id)).FirstOrDefault();

            return rtnVal;
        }

        public void CreateUser(UserModel user)
        {

        }
    }
}
