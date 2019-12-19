using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Notice.Core;
using Notice.Data.Core;
using Notice.Service;

namespace Notice.Controllers
{
    [Custom_Authorize]
    public class FrameController : BaseController
    {
        UserService userService;

        protected override void InitController()
        {
            userService = new UserService();
        }

        [Custom_Authorize(nameof(AuthType.None))]
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

        [Custom_Authorize(nameof(AuthType.None))]
        [HttpPost]
        public ActionResult Login(string userID, string password)
        {
            var user = userService.GetUser(userID);

            if (user == null)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('아이디 비밀번호를 확인해 주십시오.'); location.href = 'Login';</script>");
            }
            else
            {
                // DB 에 사용자가 있고 Password 체크
                if (password == "test")
                {
                    FormsAuthentication.SetAuthCookie(user.CN, false);

                    return RedirectToAction("Board", "Notice");
                }
                else
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('아이디 비밀번호를 확인해 주십시오.');  location.href = 'Login';</script>");
                }
            }
        }

        [Custom_Authorize(nameof(AuthType.None))]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return Redirect("Login");
        }


        public ActionResult Main()
        {
            return View();
        }
    }
}