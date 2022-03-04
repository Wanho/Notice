using Dapper;
using Notice.Data.Core;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Notice.Model;

namespace Notice.Service
{
    public class UserService
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public string GetByPasswordHash(string userID)
        {
            string passwordHash = "";
            using (var connection = new SqlConnection(connectionString))
            {
                var procedure = "[SP_UserGetByPasswordHash]";
                var values = new { UserID = userID };

                passwordHash = connection.QuerySingle<string>(procedure, values, commandType: CommandType.StoredProcedure);
            }
            return passwordHash;
        }

        public UserModel GetUser(string userID)
        {
            UserModel userModel = new UserModel();
            using (var connection = new SqlConnection(connectionString))
            {
                var procedure = "[SP_UserGetInfo]";
                var values = new { UserID = userID };

                userModel = connection.QuerySingle<UserModel>(procedure, values, commandType: CommandType.StoredProcedure);
            }
            return userModel;
        }

        public bool Verification(string userID, string password)
        {
            string passwordHash = GetByPasswordHash(userID);

            if(!string.IsNullOrEmpty(passwordHash))
            {
                var result = HashWithSlat.VerifyHashedPassword(passwordHash, password);
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

        public void CreateUser(UserModel userModel)
        {

        }

    }
}
