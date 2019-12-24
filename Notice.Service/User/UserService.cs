using Notice.Data;
using Notice.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notice.Service
{
    public class UserService
    {
        DataContext dbContext;

        public UserService()
        {
            dbContext = new DataContext();
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
