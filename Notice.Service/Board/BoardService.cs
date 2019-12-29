using Notice.Data;
using Notice.Data.Core;
using Notice.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Notice.Service
{
    public class BoardService
    {
        public class BoradDbContext : DbContext
        {
            public BoradDbContext() : base("name=DefaultConnection")
            {
                Database.SetInitializer<DataContext>(null);

                Configuration.LazyLoadingEnabled = false;
                Configuration.ProxyCreationEnabled = false;

                //Show SQL in Output Debug window.
                this.Database.Log = s =>
                {
                    System.Diagnostics.Debug.Write(s);
                    System.Diagnostics.Trace.Write(s);
                    Console.WriteLine(s);
                };

            }

            public DbSet<BoardACLModel> boardACLs { get; set; }
            public DbSet<BoardInfoModel> boardInfos { get; set; }
            public DbSet<BoardItemModel> boardItems { get; set; }
            public DbSet<BoardItemAttachModel> boardItemAttachs { get; set; }
        }

        BoradDbContext dbContext;

        public BoardService()
        {
            dbContext = new BoradDbContext();
        }

        public List<BoardInfoModel> GetBoardInfos()
        {
            List<BoardInfoModel> list = dbContext.boardInfos.ToList();

            return list;
        }

        public List<BoardItemModel> GetBoardItems(BoardSearchModel searchModel, bool totalCount = true)
        {
            List<BoardItemModel> list = new List<BoardItemModel>();

            if (totalCount)
            {
                if (!string.IsNullOrEmpty(searchModel.Area))
                {
                    if (searchModel.Area == "Title")
                    {
                        searchModel.TotalCount = dbContext.boardItems.Where(x => x.BoardID == searchModel.BoardID && x.Title.Contains(searchModel.Word)).LongCount();
                    }
                    else if(searchModel.Area == "CreateName")
                    {
                        searchModel.TotalCount = dbContext.boardItems.Where(x => x.BoardID == searchModel.BoardID && x.CreateName.Contains(searchModel.Word)).LongCount();
                    }
                }
                else
                {
                    searchModel.TotalCount = dbContext.boardItems.Where(x => x.BoardID == searchModel.BoardID).LongCount();
                }
            }

            if (!string.IsNullOrEmpty(searchModel.Area))
            {
                if (searchModel.Area == "Title")
                {
                    list = dbContext.boardItems.
                        Where(x => x.BoardID == searchModel.BoardID && x.Title.Contains(searchModel.Word))
                        .OrderBy(x => x.CreateDate)
                        .Skip(searchModel.GetRange().RowStart)
                        .Take(searchModel.GetRange().RowEnd)
                        .ToList<BoardItemModel>();
                }
                else if (searchModel.Area == "CreateName")
                {
                    list = dbContext.boardItems
                        .Where(x => x.BoardID == searchModel.BoardID && x.CreateName.Contains(searchModel.Word))
                        .OrderBy(x => x.CreateDate)
                        .Skip(searchModel.GetRange().RowStart)
                        .Take(searchModel.GetRange().RowEnd)
                        .ToList<BoardItemModel>();
                }
            }
            else
            {
                list = dbContext.boardItems
                    .Where(x => x.BoardID == searchModel.BoardID)
                    .OrderBy(x=> x.CreateDate)
                    .Skip(searchModel.GetRange().RowStart)
                    .Take(searchModel.GetRange().RowEnd)
                    .ToList<BoardItemModel>();
            }

            return list;
        }

        //public List<BoardModel> GetQueryList(BoardSearchModel searchModel, bool totalCount = true)
        //{
        //    if (totalCount)
        //    {
        //        List<SqlParameter> total_parameters = new List<SqlParameter>();

        //        #region 검색
        //        string strSql_TotalSearch = SetSearch(searchModel, ref total_parameters);
        //        #endregion

        //        string strSql_TotalCnt = @" SELECT
        //                                        count(*) TOTAL_COUNT
        //                                    FROM viewBoardDB_List AS B
        //                                    WHERE B.ExpireDate > GETDATE()
        //                                    AND B.BoardCD in (SELECT BoardCD From BoardInfo where DelYN = 'N' )
        //                                    AND B.BoardCD = @BoardCD AND B.CompanyCD = 'GRCOM'" + strSql_TotalSearch;

        //        strSql_TotalCnt = $"SELECT * FROM ( {strSql_TotalCnt } ) TBL ";
                
        //        var rtnVal = dbContext.Database.SqlQuery<int>(strSql_TotalCnt, total_parameters.ToArray() ).FirstOrDefault();

        //        searchModel.TotalCount = long.Parse(rtnVal.ToString());
        //    }

        //    List<SqlParameter> parameters = new List<SqlParameter>();

        //    #region 검색
        //    string strSql_Search = SetSearch(searchModel, ref parameters);
        //    #endregion

        //    #region 쿼리
        //    string strSql = @"
        //                        SELECT
        //                                ROW_NUMBER() OVER(ORDER BY %{SortColumn} %{SortOrder} ) as RowNum,
        //                                B.ArticleCD,
        //                                dbo.fnUserID_Idless(B.NamelessFlg, B.UserID) as UserID,
        //                                dbo.fnUserName_Nameless(B.NamelessFlg, B.UserName) as UserName,
        //                                B.BoardCD,
        //                                B.Title, 
        //                                B.SelReply,
        //                                dbo.fnFormatDateTimeStringMulti(B.CreateDate, 'kor') as UCreateDate,
        //                                B.ExpireDate,
        //                                B.ViewCnt,
        //                                B.FileOnly,
        //                                B.Importance ,
        //                                B.AttachCNT,
        //                                B.CommentCNT,
        //                                B.Ref_Seq,
        //                                B.Ref_Level,
        //                                B.Ref_Step,
        //                                B.SubCnt,
        //                                B.IsNotice,
        //                                (SELECT ISNULL(MAX(Permission), -1) from BoardACL Where ResourceCD = B.BoardCD AND ((UnitID = 'PublicID_R' and Permission = 0) OR (UnitID = 'PublicID' and Permission = 0) OR UnitID = 'wanho_kim1') and Companycd = B.Companycd) AS B_ACL, 
        //                                Gubun, 
        //                                Company, 
        //                                REPLACE(CONVERT(VARCHAR, CONVERT(MONEY, Price), 1), '.00', '') AS Price, 
        //                                Area, 
        //                                Status, 
        //                                Phone, 
        //                                Address, 
        //                                Theme, 
        //                                Point,
        //                                SortCreateDate 
        //                        FROM viewBoardDB_List AS B
        //                        WHERE B.ExpireDate > GETDATE()
        //                        AND B.BoardCD in (SELECT BoardCD From BoardInfo where DelYN = 'N' )
        //                        AND B.BoardCD = @BoardCD AND B.CompanyCD = 'GRCOM' " + strSql_Search;
        //    #endregion

        //    strSql = $"SELECT * FROM( { strSql } ) TBL WHERE TBL.RowNum > @RowStart AND TBL.RowNum <= @RowEnd ";

        //    #region 정렬

        //    strSql = SetSorting(searchModel, strSql);

        //    #endregion

        //    // 페이징
        //    parameters.Add(new SqlParameter("@RowStart", searchModel.GetRange().RowStart));
        //    parameters.Add(new SqlParameter("@RowEnd", searchModel.GetRange().RowEnd));

        //    List<BoardModel> list = dbContext.Database.SqlQuery<BoardModel>(strSql, parameters.ToArray() ).ToList();

        //    return list;
        //}

        public string SetSorting(BoardSearchModel searchModel, string strSql)
        {
            if (!string.IsNullOrEmpty(searchModel.SortColumn))
            {
                if (searchModel.SortColumn == BoardSortType.ItemID)
                {
                    strSql = strSql.Replace("%{SortColumn}", BoardSortType.ItemID);
                }
                else if (searchModel.SortColumn == BoardSortType.Company)
                {
                    strSql = strSql.Replace("%{SortColumn}", BoardSortType.Company);
                }
                else if (searchModel.SortColumn == BoardSortType.Title)
                {
                    strSql = strSql.Replace("%{SortColumn}", BoardSortType.Title);
                }
                else if (searchModel.SortColumn == BoardSortType.UserName)
                {
                    strSql = strSql.Replace("%{SortColumn}", BoardSortType.UserName);
                }
                else if (searchModel.SortColumn == BoardSortType.ViewCnt)
                {
                    strSql = strSql.Replace("%{SortColumn}", BoardSortType.ViewCnt);
                }
                else if (searchModel.SortColumn == BoardSortType.SortCreateDate)
                {
                    strSql = strSql.Replace("%{SortColumn}", BoardSortType.SortCreateDate);
                }
            }
            else
            {

                searchModel.SortColumn = BoardSortType.SortCreateDate;
                strSql = strSql.Replace("%{SortColumn}", searchModel.SortType);
            }

            if (string.IsNullOrEmpty(searchModel.SortOrder))
                searchModel.SortOrder = Data.Core.SortOrder.Desc.Value;

            strSql = strSql.Replace("%{SortOrder}", searchModel.SortOrder);

            return strSql;
        }

        public string SetSearch(BoardSearchModel searchModel, ref List<SqlParameter> parameters)
        {
            string rtnVal = "";

            parameters.Add(new SqlParameter("@BoardID", searchModel.BoardID));

            if (!string.IsNullOrEmpty(searchModel.Area))
            {
                if (searchModel.Area == "Title")
                {
                    parameters.Add(new SqlParameter("@Title", "%" + searchModel.Word + "%"));
                    rtnVal = " AND Title Like @Title";
                }
                else if (searchModel.Area == "ItemID")
                {
                    parameters.Add(new SqlParameter("@ItemID", searchModel.Word));
                    rtnVal = " AND ItemID = @ItemID";
                }
                else if (searchModel.Area == "CreateName")
                {
                    parameters.Add(new SqlParameter("@CreateName", "%" + searchModel.Word + "%"));
                    rtnVal = " AND CreateName Like @CreateName";
                }
            }

            return rtnVal;
        }
    }
}
