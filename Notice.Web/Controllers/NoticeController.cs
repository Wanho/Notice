using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notice.Data.Core;
using Notice.Service;
using Notice.Core;
using Notice.Web;
using Notice.Model;

namespace Notice.Controllers
{
    public class NoticeController : BaseController
    {
        //ILog logger = LogManager.GetLogger();

        BoardService boardService;

        protected override void InitController()
        {
            boardService = new BoardService();
        }

        public ActionResult BoardLeft()
        {
            return View(boardService.GetBoardInfos());
        }

        public ActionResult Board(BoardSearchModel searchModel)
        {
            //searchModel.SortColumn = BoardSortType.Title;

            searchModel.BoardID = "B5DF22B7-6827-4E06-B0FE-47746FAC70B9";

            if (HttpContext.Request.Cookies["hPageSize"] == null)
            {
                searchModel.PageSize = 10;
            }
            else
            {
                searchModel.PageSize = Convert.ToInt32(HttpContext.Request.Cookies["hPageSize"].Value);
            }

            List<BoardItemModel> list = boardService.GetBoardItems(searchModel);

            ContentBoardModel content = new ContentBoardModel()
            {
                boardItems = list,
                boardSearch = searchModel
            };
           
            return View(content);
        }

        public ActionResult Write()
        {
            return View();
        }

    }
}