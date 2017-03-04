using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Yifei.Services.User;

namespace Yifei.OauthService.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(string UserName, string Password)
        {
            int status = 500;
            string msg = null;
            try
            {
                if (string.IsNullOrEmpty(UserName)) throw new ArgumentNullException("UserName");
                var user = new UserService().GetUser(UserName, Password);
                if (user == null) throw new Exception("账户或密码错误！");
                var authentication = HttpContext.GetOwinContext().Authentication;
                authentication.SignIn(
                    new AuthenticationProperties(/*new Dictionary<string, string>() { { "ID", user.ID + "" } }*/)
                    {
                        IsPersistent = true, //记住账户
                    },
                        new ClaimsIdentity(new[] {
                            new Claim(ClaimsIdentity.DefaultNameClaimType, user.MA001),
                            new Claim("UserID",user.MA001+"")
                        }, "Application")
                    );
                status = 200;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { Status = status, Message = msg });
        }

        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}