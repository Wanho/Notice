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

        public UserModel GetUser(string userID)
        {
            string strSql = @"SELECT 
                                    [CN], [DisplayName], [DisplayName2], [mail]
                                    ,[Department], [Description], [Description2]
                                    ,[PhysicalDeliveryOfficeName], [Company], [Company2]
                                    ,[TitleCD], [Title], [Title2]
                                    ,[EmpNo]
                                FROM GWINSA.ezHrmaster.dbo.TBLUserMaster WHERE CN = @userID";

            var user = dbContext.Database.SqlQuery<UserModel>(strSql, new[] {new SqlParameter("@userID", userID) }).SingleOrDefault();

            return user;

        }
    }
}
