using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using Notice.Model;

using Notice.Data.Core;
using Notice.Web;

namespace Notice.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController()
        {
            this.ValidateRequest = false;
        }

        public UserState UserState { private set; get; }
        //public LanguageManager Text { private set; get; }

        protected virtual void InitController() { }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
           if (User.Identity.IsAuthenticated)
            {
                var user = UserState.LoadUser(User.Identity.Name);
                UserState = new UserState(filterContext.HttpContext, user);

                FormsIdentity id = (FormsIdentity)User.Identity;

                FormsAuthenticationTicket ticket = id.Ticket;

                var userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(ticket.UserData);
                if (userData != null)
                {
                    string language = userData.SafeGet("Lang");
                    string timeZone = userData.SafeGet("Timezone");
                    string timeZoneH = userData.SafeGet("TimezoneH");

                    if (!string.IsNullOrEmpty(language))
                    {
                        //UserState.Language = (LanguageType)language.ToPascalCase();
                    }
                    if (!string.IsNullOrEmpty(timeZone))
                    {
                        //UserState.Entity.Timezone = timeZone;
                    }
                    if (!string.IsNullOrEmpty(timeZoneH))
                    {
                        //UserState.TimeZoneH = timeZoneH;
                    }
                }
            }
            else
            {
                UserState = new UserState(filterContext.HttpContext, new User());
            }

            //Text = new LanguageManager(UserState.Language);

            Custom_AuthorizeAttribute attr = null;

            var attrlist = (Custom_AuthorizeAttribute[])filterContext.ActionDescriptor.GetCustomAttributes(typeof(Custom_AuthorizeAttribute), true);
            if (attrlist.Length == 0)
            {
                attrlist = (Custom_AuthorizeAttribute[])filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(Custom_AuthorizeAttribute), true);
                if (attrlist.Length == 0)
                {
                    attr = new Custom_AuthorizeAttribute();
                    attr.AuthType = AuthType.None;
                }
                else
                {
                    attr = attrlist[0];
                }
            }
            else
            {
                attr = attrlist[0];
            }

            bool requireAuth = attr.AuthType == AuthType.User || attr.AuthType == AuthType.Admin;
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (requireAuth)
            {
                if (User.Identity.IsAuthenticated)
                {
                    //세션 체크, 상태 보정
                    if (!filterContext.IsChildAction)
                    {
                    }

                    if (attr.AuthType == AuthType.Admin)
                    {
                        filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
                    }
                }
                else
                {
                    TempData["RedirectUrl"] = this.Request.RawUrl;
                    filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
                }
            }

            base.OnAuthorization(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["_User"] = this.UserState;
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