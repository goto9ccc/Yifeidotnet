using Microsoft.Practices.Unity;
using System.Web;
using System.Web.Http;
using Yifei.Framework.Common;
using Yifei.Services.User;

namespace YifeiMvc.Controllers.Services
{
    [RoutePrefix("Service/User")]
    public class LoginController : BaseApiController
    {
        [Dependency]
        public UserService userService { get; set; }

        [Route("Login")]
        [HttpPost]
        public object Index(string username, string password)
        {
            bool status = false;
            string msg = "登录失败";
            if (userService.Login(username, password))
            {
                HttpContext.Current.Session["username"] = username;
                HttpContext.Current.Session["userid"] = username;
                status = true;
            }
            var result = new { status = status, msg = msg };
            return result;
        }

    }
}
