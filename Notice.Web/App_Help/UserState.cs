using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Notice.Data.Core;
using Notice.Model;
using Notice.Service;

namespace Notice.Web
{
    public class UserState 
    {
        protected HttpContextBase httpContext { set; get; }
        protected HttpCookie cookies { set; get; }
        public UserModel userModel { get; set; }

        public UserState(HttpContextBase context, UserModel user)
        {
            if(user == null)
                return;

            userModel = user;
            httpContext = context;
            cookies = httpContext.Request.Cookies[Const.Name];

            if (cookies == null)
            {
                cookies = new HttpCookie(Const.Name);
                cookies.HttpOnly = true;
                cookies.Domain = Config.CookieDomain;

                httpContext.Request.Cookies.Add(cookies);
            }

            if (string.IsNullOrEmpty(userModel.UserID)) return;
        }

        public static UserModel LoadUser(string userId)
        {
            UserService service = new UserService();

            var user = service.GetById(userId);

            return user;
        }

        public static implicit operator UserModel(UserState state)
        {
            return state.userModel;
        }

        protected string GetCookie(string key, string defaultVal = null)
        {
            if (httpContext.Request.Cookies[key] == null)
            {
                return defaultVal;
            }
            else
            {
                return HttpUtility.UrlDecode(httpContext.Request.Cookies[key].Value);
            }
        }

        protected void SetCookie(string key, string value, string domain = null)
        {
            value = value ?? string.Empty;

            var cookie = httpContext.Request.Cookies[key] ?? new HttpCookie(key);
            cookie.Value = value;
            cookie.HttpOnly = true;

            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = Config.CookieDomain;
            }

            httpContext.Response.Cookies.Add(cookie);
        }

        public string SiteUrl
        {
            get { return GetCookie(nameof(SiteUrl)) ?? string.Empty; }
            set { SetCookie(nameof(SiteUrl), value, Config.CookieDomain); }
        }

        public bool Office
        {
            get { return (GetCookie(nameof(Office)) ?? Const.False) == Const.True; }
            set { SetCookie(nameof(Office), value ? Const.True : Const.False, Config.CookieDomain); }
        }

        public bool IsAuthenticated
        {
            get { return httpContext.User.Identity.IsAuthenticated; }
        }
    }
}