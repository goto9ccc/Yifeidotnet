using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Yifei.Framework.Exceptions;

namespace Yifei.Framework.Attrbutes
{
    public class ApiAuthAttribute : AuthorizationFilterAttribute
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

        public ApiAuthAttribute(bool isAuth, params string[] permissionCode)
        {
            _isAuth = isAuth;
            _permissionCode = permissionCode;
        }

        public ApiAuthAttribute()
        {
            _isAuth = true;
            _permissionCode = new string[0];
        }


        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (_isLog)
            {
                var strContrller =
                    (string)HttpContext.Current.Request.RequestContext.RouteData.Values["controller"];
                var strAction =
                    (string)HttpContext.Current.Request.RequestContext.RouteData.Values["action"];
            }
            //未进行权限检查
            if (HttpContext.Current.Session == null || string.IsNullOrEmpty(HttpContext.Current.Session["username"] as string))
            {
                throw new LoginException("未登录");
            }
        }



    }
}
