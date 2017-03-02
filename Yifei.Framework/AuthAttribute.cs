using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using Yifei.Framework.Exceptions;

namespace Yifei.Framework
{
    public class AuthAttribute: AuthorizeAttribute
    {

        /// <summary>
        /// 是否开启验证
        /// </summary>
        private readonly bool _isAuth = false;
        private readonly bool _isLog = true;


        /// <summary>
        /// 权限因子
        /// </summary>
        private readonly string[] _permissionCode;

        public AuthAttribute(bool isAuth, params string[] permissionCode)
        {
            _isAuth = isAuth;
            _permissionCode = permissionCode;
        }

        public AuthAttribute()
        {
            _isAuth = true;
            _permissionCode = new string[0];
        }




        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (_isLog)
            {
                var strContrller =
                    (string)HttpContext.Current.Request.RequestContext.RouteData.Values["controller"];
                var strAction =
                    (string)HttpContext.Current.Request.RequestContext.RouteData.Values["action"];
            }

            if (HttpContext.Current.Session == null || string.IsNullOrEmpty(HttpContext.Current.Session["username"] as string))
            {
                httpContext.Response.StatusCode = 403;
                return false;

            }
            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            //再执行自定义处理
            if (filterContext.HttpContext.Response.StatusCode == 403)
                filterContext.Result = new RedirectResult("~/User");
        }

    }
}
