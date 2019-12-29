using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notice.Model
{
    [Table("TB_BoardACL")]
    public class BoardACLModel
    {
        [Key]
        [Column(Order = 1)]
        public string BoardID { get; set; }
        [Key]
        [Column(Order = 2)]
        public string AccessID { get; set; }
        public string AccessName { get; set; }
        public string AccessLevel { get; set; }
        public string ParentBoardID { get; set; }
        public string IsAccess { get; set; }
        public string IsView { get; set; }
        public string IsWrite { get; set; }
        public string IsReply { get; set; }
        public string IsDelete { get; set; }
    }

    [Table("TB_BoardInfo")]
    public class BoardInfoModel
    {
        [Key]
        public string BoardID { get; set; }
        public string BoardName { get; set; }
        public string ParentBoardID { get; set; }
        public string BoardDescription { get; set; }
        public string AttachsizeLimit { get; set; }
        public string BoardType { get; set; }

        public virtual ICollection<BoardACLModel> BoardACLs { get; set; }

        [ForeignKey("ParentBoardID")]
        public virtual BoardInfoModel Parent { get; set; }
        public virtual ICollection<BoardInfoModel> Childs { get; set; }
    }

    [Table("TB_BoardItem")]
    public class BoardItemModel
    {
        [Key]
        public string ItemID { get; set; }
        public string BoardID { get; set; }
        public string ParentItemID { get; set; }
        public string Importance { get; set; }
        public int ReadCount { get; set; }
        public string Attachments { get; set; }
        public string IsNotice { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreateID { get; set; }
        public string CreateName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public virtual ICollection<BoardItemAttachModel> BoardItemAttaches { get; set; }
    }

    [Table("TB_BoardItemAttach")]
    public class BoardItemAttachModel
    {
        [Key]
        [Column(Order = 1)]
        public string ItemID { get; set; }
        [Key]
        [Column(Order = 2)]
        public string FileID { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get; set; }
        public string FileName { get; set; }
    }

    #region 주석
    /*
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
    */
    #endregion
}
