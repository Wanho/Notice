using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using Notice.Data.Core;
using Notice.Web;
using System.Web;
using Notice.Service;
using Notice.Model;

namespace Notice.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController()
        {
            this.ValidateRequest = false;
        }

        public UserModel userModel { private set; get; }

        protected virtual void InitController() { }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
           if (User.Identity.IsAuthenticated)
            {
                UserService service = new UserService();

                userModel = service.GetUser(User.Identity.Name);

                if(userModel == null)
                {
                    filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
                }
            }
            else
            {
                userModel = new UserModel();
            }

            base.OnAuthorization(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["_User"] = this.userModel;
            //ViewData["_Text"] = Text;

            InitController();

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }

        protected string GetDefaultViewName()
        {
            if (RouteData.DataTokens["area"] == null)
            {
                return string.Format("~/Views/{0}/{1}.cshtml", RouteData.Values["controller"], RouteData.Values["action"]);
            }
            else
            {
                return string.Format("~/Areas/{0}/Views/{1}/{2}.cshtml", RouteData.DataTokens["area"], RouteData.Values["controller"], RouteData.Values["action"]);
            }
        }
    }
}