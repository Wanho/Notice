using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notice.Model
{
    public class BoardModel
    {
        public int ArticleCD { get; set; }
        public long RowNum { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string BoardCD { get; set; }
        public string Title { get; set; }
        public string SelReply { get; set; }
        public string UCreateDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int ViewCnt { get; set; }
        public string FileOnly { get; set; }
        public int Importance { get; set; }
        public int AttachCNT { get; set; }
        public int CommentCNT { get; set; }
        public int Ref_Seq { get; set; }
        public int Ref_Level { get; set; }
        public int Ref_Step { get; set; }
        public int SubCnt { get; set; }
        public string IsNotice { get; set; }
        public int B_ACL { get; set; }
        public string Gubun { get; set; }
        public string Company { get; set; }
        public string Price { get; set; }
        public string Area { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Theme { get; set; }
        public int Point { get; set; }
        public string UCStatus { get; set; }
        public string TelOffice { get; set; }
        public string Mobile { get; set; }
    }

    public class BoardBase
    {
        [Key]
        public string CompanyCD { get; set; }
        public string BoardRootCD { get; set; }
        public string BoardCD { get; set; }
        public int ArticleCD { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserDept { get; set; }
        public string UserTitle { get; set; }
        public string NamelessFlg { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int ViewCnt { get; set; }
        public string FileOnly { get; set; }
        public int Importance { get; set; }
        public int Ref_Seq { get; set; }
        public int Ref_Level { get; set; }
        public int Ref_Step { get; set; }
        public string IsNotice { get; set; }
        public string searchauth { get; set; }
        public string Gubun { get; set; }
        public string Company { get; set; }
        public string Price { get; set; }
        public string Area { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Theme { get; set; }
        public int Point { get; set; }
        public string Parking { get; set; }
        public string NickName { get; set; }
        public string SelReply { get; set; }
    }

    
}
