using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notice.Model
{
    [Table("TB_User")]
    public class User
    {
        [Key]
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime CreateDate { get; set; }
        public string IsAdmin { get; set; } = Const.False;
    }

    public class UserModel
    {
           public string ID { get; set; }
           public string password { get; set; }
    }
}
