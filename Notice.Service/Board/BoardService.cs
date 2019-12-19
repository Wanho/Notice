using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using Notice.Model;
using Notice.Data;
using Notice.Data.Core;
using Notice.Core;
using System.Data.SqlClient;
using System.Data;
using System.Xml;

namespace Notice.Service
{
    public class BoardService
    {
        DataContext dbContext;

        public BoardService()
        {
            dbContext = new DataContext();
        }

        public List<BoardModel> GetQueryList(BoardSearchModel searchModel, bool totalCount = true)
        {
            if (totalCount)
            {
                List<SqlParameter> total_parameters = new List<SqlParameter>();

                #region 검색
                string strSql_TotalSearch = SetSearch(searchModel, ref total_parameters);
                #endregion

                string strSql_TotalCnt = @" SELECT
                                                count(*) TOTAL_COUNT
                                            FROM viewBoardDB_List AS B
                                            WHERE B.ExpireDate > GETDATE()
                                            AND B.BoardCD in (SELECT BoardCD From BoardInfo where DelYN = 'N' )
                                            AND B.BoardCD = @BoardCD AND B.CompanyCD = 'GRCOM'" + strSql_TotalSearch;

                strSql_TotalCnt = $"SELECT * FROM ( {strSql_TotalCnt } ) TBL ";
                
                var rtnVal = dbContext.Database.SqlQuery<int>(strSql_TotalCnt, total_parameters.ToArray() ).FirstOrDefault();

                searchModel.TotalCount = long.Parse(rtnVal.ToString());
            }

            List<SqlParameter> parameters = new List<SqlParameter>();

            #region 검색
            string strSql_Search = SetSearch(searchModel, ref parameters);
            #endregion

            #region 쿼리
            string strSql = @"
                                SELECT
                                        ROW_NUMBER() OVER(ORDER BY %{SortColumn} %{SortOrder} ) as RowNum,
                                        B.ArticleCD,
                                        dbo.fnUserID_Idless(B.NamelessFlg, B.UserID) as UserID,
                                        dbo.fnUserName_Nameless(B.NamelessFlg, B.UserName) as UserName,
                                        B.BoardCD,
                                        B.Title, 
                                        B.SelReply,
                                        dbo.fnFormatDateTimeStringMulti(B.CreateDate, 'kor') as UCreateDate,
                                        B.ExpireDate,
                                        B.ViewCnt,
                                        B.FileOnly,
                                        B.Importance ,
                                        B.AttachCNT,
                                        B.CommentCNT,
                                        B.Ref_Seq,
                                        B.Ref_Level,
                                        B.Ref_Step,
                                        B.SubCnt,
                                        B.IsNotice,
                                        (SELECT ISNULL(MAX(Permission), -1) from BoardACL Where ResourceCD = B.BoardCD AND ((UnitID = 'PublicID_R' and Permission = 0) OR (UnitID = 'PublicID' and Permission = 0) OR UnitID = 'wanho_kim1') and Companycd = B.Companycd) AS B_ACL, 
                                        Gubun, 
                                        Company, 
                                        REPLACE(CONVERT(VARCHAR, CONVERT(MONEY, Price), 1), '.00', '') AS Price, 
                                        Area, 
                                        Status, 
                                        Phone, 
                                        Address, 
                                        Theme, 
                                        Point,
                                        SortCreateDate 
                                FROM viewBoardDB_List AS B
                                WHERE B.ExpireDate > GETDATE()
                                AND B.BoardCD in (SELECT BoardCD From BoardInfo where DelYN = 'N' )
                                AND B.BoardCD = @BoardCD AND B.CompanyCD = 'GRCOM' " + strSql_Search;
            #endregion

            strSql = $"SELECT * FROM( { strSql } ) TBL WHERE TBL.RowNum > @RowStart AND TBL.RowNum <= @RowEnd ";

            #region 정렬

            strSql = SetSorting(searchModel, strSql);

            #endregion

            // 페이징
            parameters.Add(new SqlParameter("@RowStart", searchModel.GetRange().RowStart));
            parameters.Add(new SqlParameter("@RowEnd", searchModel.GetRange().RowEnd));

            List<BoardModel> list = dbContext.Database.SqlQuery<BoardModel>(strSql, parameters.ToArray() ).ToList();

            return list;
        }

        public string SetSorting(BoardSearchModel searchModel, string strSql)
        {
            if (!string.IsNullOrEmpty(searchModel.SortColumn))
            {
                if (searchModel.SortColumn == BoardSortType.ArticleCD)
                {
                    strSql = strSql.Replace("%{SortColumn}", BoardSortType.ArticleCD);
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

            parameters.Add(new SqlParameter("@BoardCD", searchModel.BoardCD));

            if (!string.IsNullOrEmpty(searchModel.Area))
            {
                if (searchModel.Area == "Title")
                {
                    parameters.Add(new SqlParameter("@Title", "%" + searchModel.Word + "%"));
                    rtnVal = " AND B.Title Like @Title";
                }
                else if (searchModel.Area == "ArticleCD")
                {
                    parameters.Add(new SqlParameter("@ArticleCD", searchModel.Word));
                    rtnVal = " AND B.ArticleCD = @ArticleCD";
                }
                else if (searchModel.Area == "UserName")
                {
                    parameters.Add(new SqlParameter("@UserName", "%" + searchModel.Word + "%"));
                    rtnVal = " AND B.UserName Like @UserName";
                }
            }

            return rtnVal;
        }
    }
}
