using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yifei.Services.User;

namespace YifeiMvc.Controllers
{
    public class UserController : Controller
    {
        [Dependency]
        public UserService userService { get; set; }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username,string password)
        {
            if (userService.Login(username,password))
            {
                Response.Write("<Script>alert('正确');</Script>");
            }
            else
            {
                Response.Write("<Script>alert('错误');</Script>");
            }

            return View();
        }
    }
}