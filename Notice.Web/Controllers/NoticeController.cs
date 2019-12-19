using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notice.Data.Core;
using Notice.Service;
using Notice.Model;
using Notice.Core;
using Notice.Web;

namespace Notice.Controllers
{
    [Custom_Authorize]
    public class NoticeController : BaseController
    {
        ILog logger = LogManager.GetLogger();

        BoardService boardService;

        protected override void InitController()
        {
            boardService = new BoardService();
        }

        [Custom_Authorize(nameof(AuthType.User))]
        public ActionResult Board(BoardSearchModel searchModel)
        {
            searchModel.BoardCD = "418818";

            if (HttpContext.Request.Cookies["hPageSize"] == null) {
                searchModel.PageSize = 10;
            }
            else {
                searchModel.PageSize = Convert.ToInt32(HttpContext.Request.Cookies["hPageSize"].Value);
            }

            List<BoardModel> queryList = boardService.GetQueryList(searchModel);

            ContentBoardModel content = new ContentBoardModel()
            {
                boardModel = queryList,
                boardSearchModel = searchModel
            };
            
            return View(content);
        }

        [Custom_Authorize(nameof(AuthType.User))]
        public ActionResult Write()
        {
            return View();
        }

    }
}