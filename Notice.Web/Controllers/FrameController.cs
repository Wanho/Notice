using Notice.Data.Core;
using Notice.Service;
using Notice.Model;
using System.Web.Mvc;
using System.Web.Security;
using log4net;

namespace Notice.Controllers
{
    public class FrameController : BaseController
    {
        UserService userService;

        protected override void InitController()
        {
            userService = new UserService();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Board", "Notice");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(string userID, string password)
        {
            if(userService.Verification(userID, password))
            {
                FormsAuthentication.SetAuthCookie(userID, false);

                return RedirectToAction("Board", "Notice");
            }
            else
            {
                return Content("<script language='javascript' type='text/javascript'>alert('아이디 비밀번호를 확인해 주십시오.'); location.href = 'Login';</script>");
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return Redirect("Login");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserModel userModel)
        {
            userService.CreateUser(userModel);

            return new EmptyResult();
        }

        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}