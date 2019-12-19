using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notice.Data.Core;
using Notice.Service;
using Notice.Model;
using Notice.Core;

namespace Notice.Controllers
{
    public class HomeTestController : BaseController
    {
        ILog logger = LogManager.GetLogger();

        BoardService boardService;

        protected override void InitController()
        {
            boardService = new BoardService();
        }

        public ActionResult Index(BoardSearchModel searchModel)
        {
            logger.Error("HomeController", "이건 에러");
            logger.Info("HomeController", "이건 정보");
            logger.Debug("HomeController", "이건 디버그");
            logger.Fatal("HomeController", "이건 치명적");
            logger.Warn("HomeController", "이건 경고");

            string protocol = Protocol.Http;

            if (Protocol.Http.Value == "http")
            {
                Console.Write("Test");
            }

            if (Protocol.Http == (Protocol)protocol)
            {
                Console.Write("Test");
            }

            WebPath dirAttachFile = FileType.Attach.GetDirectoryPath("wanho_kim1", DateTime.UtcNow);

            Console.Write(dirAttachFile.Path);
            Console.Write(dirAttachFile.AbsolutePath);

            //List<BoardBase> list = boardService.GetList();

            searchModel = new BoardSearchModel();
            searchModel.BoardCD = "418818";
            searchModel.Title = "";
            searchModel.PageNumber = 1;
            searchModel.GetRange();

            //searchModel = boardService.GetQueryList(searchModel, true);

            List<BoardModel> queryList = boardService.GetQueryList(searchModel);
             
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}